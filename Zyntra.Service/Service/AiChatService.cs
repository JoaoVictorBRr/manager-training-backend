using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Zyntra.Domain.Dtos.AiChatDto;
using Zyntra.Domain.Dtos.DietDto;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class AiChatService(
    IAiChatRepository repo,
    IStudentService studentService,
    IWorkoutSheetService workoutSheetService,
    IStudentDietService dietService,
    IWorkoutTemplateService workoutTemplateService,
    IExerciseService exerciseService,
    HttpClient httpClient,
    IConfiguration configuration) : IAiChatService
{
    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    // ─── Public methods ──────────────────────────────────────────────────────

    public async Task<AiChatMessageResponseDto> SendMessageAsync(long userId, long studentId, string userMessage)
    {
        var student = await studentService.GetByUserIdAsync(userId)
            ?? throw new InvalidOperationException("Aluno não encontrado.");

        var onboarding = student.OnboardingDataJson != null
            ? JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson, _jsonOpts)
            : null;

        WorkoutSheet? workout = null;
        try { workout = await workoutSheetService.GetActiveByStudentAsync(studentId); }
        catch { /* no active workout */ }

        var diet = await dietService.GetActiveDietAsync(studentId);

        // Save user message
        var userMsg = new AiChatMessage
        {
            StudentId = studentId,
            Role = "user",
            Content = userMessage,
        };
        await repo.AddAsync(userMsg);

        // Fetch recent history for context (excluding the message just saved)
        var history = (await repo.GetHistoryAsync(studentId, 40)).ToList();

        var systemPrompt = BuildSystemPrompt(student, onboarding, workout, diet);
        var messages = BuildMessages(systemPrompt, history, userMessage);

        var aiRaw = await CallOpenAiAsync(messages);
        var (cleanContent, actionJson) = ParseResponse(aiRaw);

        var aiMsg = new AiChatMessage
        {
            StudentId = studentId,
            Role = "assistant",
            Content = cleanContent,
            ActionJson = actionJson,
            ActionStatus = actionJson != null ? "pending" : "none",
        };
        await repo.AddAsync(aiMsg);

        return MapToDto(aiMsg);
    }

    public async Task<IEnumerable<AiChatMessageResponseDto>> GetHistoryAsync(long studentId)
    {
        var msgs = await repo.GetHistoryAsync(studentId, 100);
        return msgs.Select(MapToDto);
    }

    public async Task ConfirmActionAsync(long messageId, long studentId)
    {
        var msg = await repo.GetByIdAsync(messageId)
            ?? throw new InvalidOperationException("Mensagem não encontrada.");

        if (msg.StudentId != studentId)
            throw new UnauthorizedAccessException("Acesso negado.");

        if (msg.ActionStatus != "pending" || msg.ActionJson == null)
            throw new InvalidOperationException("Nenhuma ação pendente nesta mensagem.");

        var action = JsonSerializer.Deserialize<AiChatActionDto>(msg.ActionJson, _jsonOpts)
            ?? throw new InvalidOperationException("Ação inválida.");

        await ExecuteActionAsync(action, studentId);

        msg.ActionStatus = "confirmed";
        await repo.UpdateAsync(msg);
    }

    public async Task RejectActionAsync(long messageId, long studentId)
    {
        var msg = await repo.GetByIdAsync(messageId)
            ?? throw new InvalidOperationException("Mensagem não encontrada.");

        if (msg.StudentId != studentId)
            throw new UnauthorizedAccessException("Acesso negado.");

        msg.ActionStatus = "rejected";
        await repo.UpdateAsync(msg);
    }

    // ─── System prompt ───────────────────────────────────────────────────────

    private static string BuildSystemPrompt(
        Student student,
        SaveOnboardingDto? o,
        WorkoutSheet? workout,
        StudentDietResponseDto? diet)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Você é Zyntra, assistente pessoal de fitness integrado ao aplicativo de academia.");
        sb.AppendLine();

        sb.AppendLine("PERFIL DO ALUNO:");
        sb.AppendLine($"- Nome: {student.User?.Name ?? "Aluno"}");
        sb.AppendLine($"- Objetivo: {TranslateObjective(o?.Objective)}");
        sb.AppendLine($"- Idade: {o?.Age ?? 0} anos, Peso: {o?.Weight ?? 0}kg, Altura: {o?.Height ?? 0}cm");
        sb.AppendLine($"- Dias de treino: {o?.TrainingDays ?? 4}x/semana | Duração: {o?.TrainingDuration ?? "1hour"}");
        sb.AppendLine($"- Horário preferido: {TranslateTime(o?.PreferredTime)} | Ambiente: {TranslateEnvironment(o?.Environment)}");
        sb.AppendLine($"- Restrições alimentares: {(o?.DietRestrictions?.Count > 0 ? string.Join(", ", o.DietRestrictions) : "nenhuma")}");
        sb.AppendLine($"- Lesões/limitações: {(o?.Injuries?.Count > 0 ? string.Join(", ", o.Injuries) : "nenhuma")}");
        sb.AppendLine();

        if (workout != null)
        {
            sb.AppendLine($"TREINO ATUAL (SheetId: {workout.Id}):");
            var exercisesByDay = (workout.Exercises ?? [])
                .GroupBy(e => e.WorkoutDay)
                .OrderBy(g => g.Key);

            foreach (var day in exercisesByDay)
            {
                var exerciseList = string.Join(" | ", day.Select(e => $"[ID:{e.Id}] {e.Name} ({e.MuscleGroup}) - {e.Sets}x{e.Repetitions}"));
                sb.AppendLine($"Dia {day.Key}: {exerciseList}");
            }
        }
        else
        {
            sb.AppendLine("TREINO ATUAL: nenhum treino ativo.");
        }

        sb.AppendLine();

        if (diet != null && diet.Meals.Count > 0)
        {
            sb.AppendLine($"DIETA ATUAL (Total: {diet.TotalCalories}kcal/dia):");
            foreach (var meal in diet.Meals)
            {
                var opts = meal.Options.Count > 0
                    ? string.Join("; ", meal.Options.Select(op => $"{op.FoodName} {op.Quantity} ({op.Calories}kcal, {op.Protein}g prot)"))
                    : "sem opções";
                sb.AppendLine($"{meal.MealTypeName}: {opts}");
            }
        }
        else
        {
            sb.AppendLine("DIETA ATUAL: nenhuma dieta ativa.");
        }

        sb.AppendLine();
        sb.AppendLine("INSTRUÇÕES:");
        sb.AppendLine("- Responda SEMPRE em português do Brasil");
        sb.AppendLine("- Seja motivador, específico e acolhedor");
        sb.AppendLine("- Respeite sempre o objetivo, limitações e preferências do aluno");
        sb.AppendLine("- Só proponha ações se o aluno pedir explicitamente uma mudança");
        sb.AppendLine("- Para propor uma ação, finalize sua resposta com exatamente: [ACTION]{json}[/ACTION]");
        sb.AppendLine("- Tipos de ação e seus params:");
        sb.AppendLine("  REGENERATE_WORKOUT: {\"type\":\"REGENERATE_WORKOUT\",\"label\":\"Regenerar treino\",\"params\":{\"trainingDays\":N}}");
        sb.AppendLine("  REPLACE_EXERCISE: {\"type\":\"REPLACE_EXERCISE\",\"label\":\"Substituir X por Y\",\"params\":{\"deleteExerciseId\":N,\"newExercise\":{\"workoutSheetId\":N,\"name\":\"...\",\"muscleGroup\":\"...\",\"sets\":N,\"repetitions\":\"12\",\"workoutDay\":\"A\",\"exerciseType\":1}}}");
        sb.AppendLine("  CREATE_DIET: {\"type\":\"CREATE_DIET\",\"label\":\"Criar novo plano alimentar\",\"params\":{\"name\":\"Plano ...\",\"meals\":[{\"mealType\":1,\"options\":[{\"foodName\":\"...\",\"quantity\":\"...\",\"calories\":N,\"protein\":N,\"carbs\":N,\"fat\":N}]}]}}");
        sb.AppendLine("  (mealType: 1=Café da Manhã, 2=Almoço, 3=Café da Tarde, 4=Janta, 5=Ceia)");
        sb.AppendLine("  (exerciseType: 1=musculação, 2=cardio, 3=peso corporal)");
        sb.AppendLine("- Só inclua [ACTION] quando houver uma proposta concreta de mudança");

        return sb.ToString();
    }

    // ─── OpenAI call ─────────────────────────────────────────────────────────

    private async Task<string> CallOpenAiAsync(List<object> messages)
    {
        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API key não configurada.");
        var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        var body = JsonSerializer.Serialize(new
        {
            model,
            messages,
            max_tokens = 1200,
            temperature = 0.7,
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAI API error: {response.StatusCode} — {responseBody}");

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    private static List<object> BuildMessages(string systemPrompt, List<AiChatMessage> history, string newUserMessage)
    {
        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };

        // Add history (skip the last entry which is the user message we just saved)
        foreach (var msg in history.SkipLast(1))
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = newUserMessage });
        return messages;
    }

    // ─── Response parsing ────────────────────────────────────────────────────

    private static (string content, string? actionJson) ParseResponse(string raw)
    {
        var match = Regex.Match(raw, @"\[ACTION\](.*?)\[/ACTION\]", RegexOptions.Singleline);
        if (!match.Success)
            return (raw.Trim(), null);

        var actionJson = match.Groups[1].Value.Trim();
        var cleanContent = raw.Replace(match.Value, string.Empty).Trim();
        return (cleanContent, actionJson);
    }

    // ─── Action execution ─────────────────────────────────────────────────────

    private async Task ExecuteActionAsync(AiChatActionDto action, long studentId)
    {
        switch (action.Type)
        {
            case "REGENERATE_WORKOUT":
                var trainingDays = action.Params.GetProperty("trainingDays").GetInt32();
                await workoutTemplateService.GenerateWorkoutAsync(studentId, trainingDays);
                break;

            case "REPLACE_EXERCISE":
                var deleteId = action.Params.GetProperty("deleteExerciseId").GetInt64();
                await exerciseService.DeleteAsync(deleteId);

                var newExerciseParams = action.Params.GetProperty("newExercise");
                var newExercise = new Exercise
                {
                    WorkoutSheetId = newExerciseParams.GetProperty("workoutSheetId").GetInt64(),
                    Name = newExerciseParams.GetProperty("name").GetString() ?? string.Empty,
                    MuscleGroup = newExerciseParams.GetProperty("muscleGroup").GetString() ?? string.Empty,
                    Sets = newExerciseParams.GetProperty("sets").GetInt32(),
                    Repetitions = newExerciseParams.GetProperty("repetitions").GetString() ?? "12",
                    WorkoutDay = newExerciseParams.GetProperty("workoutDay").GetString() ?? "A",
                    ExerciseType = (ExerciseType)newExerciseParams.GetProperty("exerciseType").GetInt32(),
                    VideoUrl = string.Empty,
                    Description = string.Empty,
                };
                await exerciseService.AddAsync(newExercise);
                break;

            case "CREATE_DIET":
                var createDietDto = JsonSerializer.Deserialize<CreateDietDto>(
                    action.Params.GetRawText(), _jsonOpts)
                    ?? throw new InvalidOperationException("Parâmetros de dieta inválidos.");
                await dietService.CreateDietAsync(studentId, createDietDto);
                break;

            default:
                throw new InvalidOperationException($"Tipo de ação desconhecido: {action.Type}");
        }
    }

    // ─── Mapping ─────────────────────────────────────────────────────────────

    private static AiChatMessageResponseDto MapToDto(AiChatMessage msg)
    {
        AiChatActionDto? action = null;
        if (msg.ActionJson != null)
        {
            try { action = JsonSerializer.Deserialize<AiChatActionDto>(msg.ActionJson, _jsonOpts); }
            catch { /* invalid json — skip */ }
        }

        return new AiChatMessageResponseDto
        {
            Id = msg.Id,
            Role = msg.Role,
            Content = msg.Content,
            Action = action,
            ActionStatus = msg.ActionStatus,
            CreatedAt = msg.DateCreated,
        };
    }

    // ─── Translation helpers ─────────────────────────────────────────────────

    private static string TranslateObjective(string? obj) => obj switch
    {
        "lose_weight" => "Emagrecer",
        "gain_muscle" => "Ganhar massa muscular",
        "get_toned" => "Definir corpo",
        "health" => "Melhorar saúde",
        "conditioning" => "Condicionamento físico",
        _ => "Não informado",
    };

    private static string TranslateTime(string? t) => t switch
    {
        "morning" => "Manhã",
        "afternoon" => "Tarde",
        "evening" or "night" => "Noite",
        _ => "Flexível",
    };

    private static string TranslateEnvironment(string? e) => e switch
    {
        "gym" => "Academia",
        "home" => "Casa",
        "both" => "Academia e Casa",
        _ => "Não informado",
    };
}
