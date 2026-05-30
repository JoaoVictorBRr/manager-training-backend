using System.Text;
using System.Text.Json;
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
    IExerciseService exerciseService) : IAiChatService
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

        var userMsg = new AiChatMessage
        {
            StudentId = studentId,
            Role = "user",
            Content = userMessage,
        };
        await repo.AddAsync(userMsg);

        var history = (await repo.GetHistoryAsync(studentId, 20)).ToList();

        var (cleanContent, actionJson) = BuildRuleBasedResponse(userMessage, student, onboarding, workout, diet, history);

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

    // ─── Rule-based response engine ──────────────────────────────────────────

    private static (string, string?) BuildRuleBasedResponse(
        string userMessage,
        Student student,
        SaveOnboardingDto? o,
        WorkoutSheet? workout,
        StudentDietResponseDto? diet,
        List<AiChatMessage> history)
    {
        var msg = userMessage.ToLower().Trim();
        var firstName = GetFirstName(student.User?.Name);
        var lastBot = (history.LastOrDefault(h => h.Role == "assistant")?.Content ?? "").ToLower();

        // Saudações
        if (HasAny(msg, "ola", "olá", "oi ", "oi,", "^oi$", "ei ", "hey", "hello", "hi ", "bom dia", "boa tarde", "boa noite", "boas", "salve", "eai", "e aí", "e ai")
            || msg == "oi")
            return (BuildGreeting(firstName), null);

        // Seleção numerada — detecta contexto pelo último bot
        if (msg is "1" or "2" or "3" or "4")
        {
            // Menu principal
            if (HasAll(lastBot, "meu treino", "minha dieta"))
                return msg switch
                {
                    "1" => BuildTrainingMenu(firstName, workout, o),
                    "2" => BuildDietMenu(firstName, diet),
                    "3" => (BuildHealthTips(firstName, o), null),
                    "4" => (BuildMotivation(firstName, o), null),
                    _ => (BuildDefault(firstName), null),
                };

            // Submenu treino
            if (HasAny(lastBot, "regenerar meu treino", "substituir um exercício", "dicas de treino"))
                return msg switch
                {
                    "1" => BuildRegenerateWorkout(o),
                    "2" => (BuildReplaceExerciseInfo(firstName, workout), null),
                    "3" => (BuildTrainingTips(firstName, o), null),
                    _ => (BuildDefault(firstName), null),
                };

            // Submenu dieta
            if (HasAny(lastBot, "criar novo plano alimentar", "criar plano alimentar", "dicas de alimentação"))
                return msg switch
                {
                    "1" => BuildCreateDiet(firstName, o),
                    "2" => (BuildDietTips(firstName, o), null),
                    "3" => (BuildExplainDiet(firstName, diet), null),
                    _ => (BuildDefault(firstName), null),
                };

            // Submenu sem treino
            if (HasAny(lastBot, "gerar meu treino") && HasAny(lastBot, "dicas de treino"))
                return msg switch
                {
                    "1" => BuildRegenerateWorkout(o),
                    "2" => (BuildTrainingTips(firstName, o), null),
                    _ => (BuildDefault(firstName), null),
                };

            // Submenu sem dieta
            if (HasAny(lastBot, "criar plano alimentar") && HasAny(lastBot, "dicas de alimentação"))
                return msg switch
                {
                    "1" => BuildCreateDiet(firstName, o),
                    "2" => (BuildDietTips(firstName, o), null),
                    _ => (BuildDefault(firstName), null),
                };
        }

        // Regenerar treino
        if (HasAny(msg, "regenerar treino", "gerar novo treino", "novo treino", "recriar treino",
                        "criar treino", "montar treino", "quero mudar meu treino", "gera um treino", "cria um treino"))
            return BuildRegenerateWorkout(o);

        // Substituir exercício
        if (HasAny(msg, "substituir exercício", "substituir exercicio", "trocar exercício", "trocar exercicio",
                        "mudar exercício", "mudar exercicio", "quero substituir"))
            return (BuildReplaceExerciseInfo(firstName, workout), null);

        // Criar dieta
        if (HasAny(msg, "criar dieta", "nova dieta", "novo plano alimentar", "quero mudar minha dieta",
                        "criar plano", "monte uma dieta", "montar dieta", "gera uma dieta", "cria uma dieta"))
            return BuildCreateDiet(firstName, o);

        // Treino
        if (HasAny(msg, "treino", "exercício", "exercicio", "musculação", "musculacao", "academia",
                        "série", "serie", "repetição", "repeticao", "músculo", "musculo", "treinar",
                        "hipertrofia", "força", "forca", "peito", "costas", "perna", "bíceps", "biceps",
                        "tríceps", "triceps", "ombro", "abdômen", "abdomen", "glúteo", "gluteo", "panturrilha"))
            return BuildTrainingMenu(firstName, workout, o);

        // Dieta
        if (HasAny(msg, "dieta", "alimentação", "alimentacao", "comer", "comida", "refeição", "refeicao",
                        "nutrição", "nutricao", "caloria", "proteína", "proteina", "carboidrato", "gordura",
                        "emagrecer", "engordar", "nutriente", "macro", "almoço", "almoco", "janta",
                        "café da manhã", "lanche", "ceia"))
            return BuildDietMenu(firstName, diet);

        // Saúde e bem-estar
        if (HasAny(msg, "saúde", "saude", "bem-estar", "bem estar", "dormir", "sono", "hidratação",
                        "hidratacao", "água", "agua", "suplemento", "descanso", "recuperação", "recuperacao",
                        "lesão", "lesao", "dor", "alongamento", "aquecimento"))
            return (BuildHealthTips(firstName, o), null);

        // Motivação e resultados
        if (HasAny(msg, "motivação", "motivacao", "motivado", "progresso", "resultado", "evolução",
                        "evolucao", "conseguir", "difícil", "dificil", "cansado", "desanimado",
                        "ânimo", "animo", "persistir", "desistir", "não consigo", "nao consigo"))
            return (BuildMotivation(firstName, o), null);

        return (BuildDefault(firstName), null);
    }

    // ─── Response builders ────────────────────────────────────────────────────

    private static string BuildGreeting(string firstName) =>
        $"Olá, {firstName}! Fico feliz em te ver por aqui 😊\n\n" +
        "O que você deseja fazer hoje?\n\n" +
        "1️⃣ Meu treino\n" +
        "2️⃣ Minha dieta\n" +
        "3️⃣ Dicas de saúde e bem-estar\n" +
        "4️⃣ Motivação e resultados";

    private static (string, string?) BuildTrainingMenu(string firstName, WorkoutSheet? workout, SaveOnboardingDto? o)
    {
        if (workout == null)
            return (
                $"Você ainda não tem um treino ativo, {firstName}! Vamos criar um?\n\n" +
                "1️⃣ Gerar meu treino\n" +
                "2️⃣ Dicas de treino",
                null
            );

        var exerciseCount = workout.Exercises?.Count ?? 0;
        var days = workout.Exercises?.Select(e => e.WorkoutDay).Distinct().Count() ?? 0;
        var trainingDays = o?.TrainingDays ?? days;

        return (
            $"Seu treino está ativo, {firstName}! Você treina **{trainingDays}x por semana** com " +
            $"**{exerciseCount} exercícios** em **{days} dias** diferentes.\n\n" +
            "O que você quer fazer?\n\n" +
            "1️⃣ Regenerar meu treino completo\n" +
            "2️⃣ Substituir um exercício\n" +
            "3️⃣ Dicas de treino",
            null
        );
    }

    private static (string, string?) BuildDietMenu(string firstName, StudentDietResponseDto? diet)
    {
        if (diet == null || diet.Meals.Count == 0)
            return (
                $"Você ainda não tem uma dieta ativa, {firstName}! Que tal montar um plano alimentar personalizado?\n\n" +
                "1️⃣ Criar plano alimentar\n" +
                "2️⃣ Dicas de alimentação",
                null
            );

        return (
            $"Sua dieta atual tem **{diet.TotalCalories}kcal por dia**, {firstName}, " +
            $"distribuídas em **{diet.Meals.Count} refeições**.\n\n" +
            "O que você quer fazer?\n\n" +
            "1️⃣ Criar novo plano alimentar\n" +
            "2️⃣ Dicas de alimentação\n" +
            "3️⃣ Entender minha dieta atual",
            null
        );
    }

    private static string BuildHealthTips(string firstName, SaveOnboardingDto? o)
    {
        var injuryNote = o?.Injuries?.Count > 0
            ? $"\n\nComo você tem restrições ({string.Join(", ", o.Injuries)}), respeite sempre os limites do seu corpo."
            : string.Empty;

        return
            $"Dicas importantes de saúde para você, {firstName}! 💪\n\n" +
            "**Hidratação:** Beba pelo menos 35ml de água por kg de peso por dia.\n\n" +
            "**Sono:** Durma de 7 a 9 horas — é nesse período que os músculos se recuperam e crescem.\n\n" +
            "**Descanso:** Respeite os dias de descanso para evitar overtraining e lesões.\n\n" +
            "**Consistência:** Prefira frequência à intensidade extrema — resultados vêm com regularidade.\n\n" +
            "**Aquecimento:** Sempre faça 5-10 minutos de aquecimento antes do treino para proteger as articulações." +
            injuryNote;
    }

    private static string BuildMotivation(string firstName, SaveOnboardingDto? o)
    {
        var objective = o?.Objective switch
        {
            "lose_weight"    => "emagrecer",
            "gain_muscle"    => "ganhar massa muscular",
            "get_toned"      => "definir o corpo",
            "health"         => "melhorar sua saúde",
            "conditioning"   => "melhorar seu condicionamento físico",
            _                => "alcançar seus objetivos",
        };

        return
            $"Você está no caminho certo, {firstName}! 🌟\n\n" +
            $"Seu objetivo de **{objective}** é totalmente alcançável com consistência e dedicação.\n\n" +
            "**Lembre-se:** Todo progresso conta, mesmo que pequeno. Cada treino completado é uma vitória.\n\n" +
            "**Dica mental:** Não compare seu progresso com o de outros — compare com quem você era ontem.\n\n" +
            "**Foco:** Os resultados aparecem para quem persiste, não para quem treina perfeito. Continue! 💪";
    }

    private static string BuildTrainingTips(string firstName, SaveOnboardingDto? o) =>
        o?.Objective switch
        {
            "lose_weight" =>
                $"Dicas de treino para emagrecer, {firstName}! 🏃\n\n" +
                "• Priorize exercícios compostos (agachamento, supino, levantamento terra)\n" +
                "• Inclua cardio moderado após o treino de força\n" +
                "• Mantenha intervalos curtos (30-60s) para maior queima calórica\n" +
                "• Progrida a carga gradualmente para evitar adaptação",
            "gain_muscle" =>
                $"Dicas de treino para hipertrofia, {firstName}! 💪\n\n" +
                "• Foco em sobrecarga progressiva — aumente o peso gradualmente\n" +
                "• Treine cada grupo muscular 2x por semana para maior estímulo\n" +
                "• Séries de 8-12 repetições são ideais para hipertrofia\n" +
                "• Descanse 60-90 segundos entre as séries para recuperação adequada",
            _ =>
                $"Dicas gerais de treino, {firstName}! 🏋️\n\n" +
                "• Priorize a forma correta sobre a carga — evita lesões e melhora resultados\n" +
                "• Varie os exercícios a cada 4-6 semanas para evitar platôs\n" +
                "• Mantenha um diário de treino para acompanhar sua evolução\n" +
                "• Hidrate-se durante o treino — afeta diretamente a performance",
        };

    private static string BuildReplaceExerciseInfo(string firstName, WorkoutSheet? workout)
    {
        if (workout?.Exercises == null || workout.Exercises.Count == 0)
            return $"{firstName}, você ainda não tem exercícios no treino para substituir. Quer gerar um treino primeiro?";

        var list = workout.Exercises
            .Take(10)
            .Select(e => $"• **{e.Name}** ({e.MuscleGroup}) — Dia {e.WorkoutDay}, {e.Sets}x{e.Repetitions}");

        var extra = workout.Exercises.Count > 10
            ? $"\n... e mais {workout.Exercises.Count - 10} exercícios."
            : string.Empty;

        return
            $"Para substituir um exercício, {firstName}, me informe qual você quer trocar e por qual gostaria de substituir!\n\n" +
            "**Seus exercícios atuais:**\n" +
            string.Join("\n", list) +
            extra +
            "\n\nEx: *\"Quero substituir Supino Reto por Crucifixo\"*";
    }

    private static (string, string?) BuildRegenerateWorkout(SaveOnboardingDto? o)
    {
        var days = o?.TrainingDays ?? 4;
        var json = $"{{\"type\":\"REGENERATE_WORKOUT\",\"label\":\"Regenerar treino completo\",\"params\":{{\"trainingDays\":{days}}}}}";

        return (
            $"Vou regenerar seu treino completo com base no seu perfil! O novo plano será montado para " +
            $"**{days} dias por semana**.\n\nConfirme abaixo para aplicar:",
            json
        );
    }

    private static (string, string?) BuildCreateDiet(string firstName, SaveOnboardingDto? o)
    {
        var isVeg = o?.DietRestrictions?.Any(r =>
            r.Contains("vegetarian", StringComparison.OrdinalIgnoreCase) ||
            r.Contains("vegano", StringComparison.OrdinalIgnoreCase)) ?? false;

        var (name, mealsJson, totalCal) = (o?.Objective ?? "health") switch
        {
            "lose_weight" => LoseWeightDietJson(isVeg),
            "gain_muscle" => GainMuscleDietJson(isVeg),
            _             => BalancedDietJson(isVeg),
        };

        var actionJson = $"{{\"type\":\"CREATE_DIET\",\"label\":\"Criar plano alimentar\",\"params\":{{\"name\":\"{name}\",\"meals\":{mealsJson}}}}}";

        return (
            $"Criei um plano alimentar personalizado para você, {firstName}! 🥗\n\n" +
            $"**{name}** — aproximadamente **{totalCal}kcal/dia**\n\n" +
            "Baseado no seu objetivo e perfil. Confirme para aplicar:",
            actionJson
        );
    }

    private static string BuildDietTips(string firstName, SaveOnboardingDto? o) =>
        o?.Objective switch
        {
            "lose_weight" =>
                $"Dicas de alimentação para emagrecer, {firstName}! 🥗\n\n" +
                "• Crie um déficit calórico moderado de 300-500kcal abaixo do seu gasto total\n" +
                "• Priorize proteínas para preservar a massa muscular (1,6-2g por kg de peso)\n" +
                "• Prefira alimentos integrais e ricos em fibras — aumentam a saciedade\n" +
                "• Evite ultraprocessados e bebidas açucaradas\n" +
                "• Coma a cada 3-4 horas para evitar picos de fome",
            "gain_muscle" =>
                $"Dicas de alimentação para ganhar massa, {firstName}! 💪\n\n" +
                "• Consuma um superávit calórico moderado de 200-400kcal acima do gasto\n" +
                "• Ingira de 1,8 a 2,5g de proteína por kg de peso corporal\n" +
                "• Não negligencie os carboidratos — são o combustível do treino\n" +
                "• Distribua as refeições ao longo do dia para síntese proteica constante\n" +
                "• Coma algo proteico até 30 minutos após o treino",
            _ =>
                $"Dicas de alimentação para você, {firstName}! 🥗\n\n" +
                "• Priorize alimentos naturais e minimamente processados\n" +
                "• Inclua proteína em todas as refeições principais\n" +
                "• Não pule refeições — pode levar a excessos depois\n" +
                "• Beba água antes das refeições para controlar o apetite\n" +
                "• Planeje suas refeições com antecedência para evitar escolhas ruins",
        };

    private static string BuildExplainDiet(string firstName, StudentDietResponseDto? diet)
    {
        if (diet == null || diet.Meals.Count == 0)
            return $"{firstName}, você ainda não tem uma dieta ativa. Quer criar um plano alimentar personalizado?";

        var sb = new StringBuilder();
        sb.AppendLine($"Resumo da sua dieta atual, {firstName}! 📊\n");
        sb.AppendLine($"**Total:** {diet.TotalCalories}kcal/dia | **{diet.Meals.Count} refeições**\n");

        foreach (var meal in diet.Meals.Take(5))
        {
            sb.AppendLine($"**{meal.MealTypeName}:**");
            if (meal.Options.Count > 0)
                foreach (var opt in meal.Options.Take(3))
                    sb.AppendLine($"• {opt.FoodName} — {opt.Quantity} ({opt.Calories}kcal, {opt.Protein}g prot)");
            else
                sb.AppendLine("• Sem opções cadastradas");
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }

    private static string BuildDefault(string firstName) =>
        $"Oi, {firstName}! Posso te ajudar com seu treino e dieta aqui na Zyntra. 💪\n\n" +
        "O que você deseja fazer?\n\n" +
        "1️⃣ Meu treino\n" +
        "2️⃣ Minha dieta\n" +
        "3️⃣ Dicas de saúde e bem-estar\n" +
        "4️⃣ Motivação e resultados\n\n" +
        "Ou fale diretamente:\n" +
        "• *\"Quero regenerar meu treino\"*\n" +
        "• *\"Preciso de dicas de alimentação\"*\n" +
        "• *\"Quero substituir um exercício\"*";

    // ─── Diet templates ───────────────────────────────────────────────────────

    private static (string name, string mealsJson, int calories) LoseWeightDietJson(bool veg)
    {
        var meals = new[]
        {
            new { mealType = 1, options = new[]
            {
                new { foodName = "Ovos mexidos", quantity = "2 unidades", calories = 140, protein = 12, carbs = 2, fat = 9 },
                new { foodName = "Pão integral", quantity = "1 fatia", calories = 70, protein = 3, carbs = 13, fat = 1 },
            }},
            new { mealType = 2, options = new[]
            {
                new { foodName = veg ? "Tofu grelhado" : "Frango grelhado", quantity = "150g", calories = 160, protein = veg ? 16 : 32, carbs = veg ? 4 : 0, fat = 5 },
                new { foodName = "Arroz integral", quantity = "4 colheres de sopa", calories = 140, protein = 3, carbs = 30, fat = 1 },
                new { foodName = "Salada verde", quantity = "à vontade", calories = 30, protein = 2, carbs = 5, fat = 0 },
            }},
            new { mealType = 3, options = new[]
            {
                new { foodName = "Iogurte grego natural", quantity = "170g", calories = 100, protein = 17, carbs = 6, fat = 0 },
                new { foodName = "Maçã", quantity = "1 unidade", calories = 80, protein = 0, carbs = 21, fat = 0 },
            }},
            new { mealType = 4, options = new[]
            {
                new { foodName = veg ? "Lentilha cozida" : "Peixe grelhado", quantity = "150g", calories = 160, protein = veg ? 13 : 30, carbs = veg ? 28 : 0, fat = 2 },
                new { foodName = "Batata doce", quantity = "100g", calories = 90, protein = 2, carbs = 21, fat = 0 },
                new { foodName = "Brócolis cozido", quantity = "100g", calories = 35, protein = 3, carbs = 7, fat = 0 },
            }},
        };
        return ("Plano para Emagrecimento", JsonSerializer.Serialize(meals), 1005);
    }

    private static (string name, string mealsJson, int calories) GainMuscleDietJson(bool veg)
    {
        var meals = new[]
        {
            new { mealType = 1, options = new[]
            {
                new { foodName = "Ovos mexidos", quantity = "3 unidades", calories = 210, protein = 18, carbs = 3, fat = 14 },
                new { foodName = "Pão integral", quantity = "2 fatias", calories = 140, protein = 6, carbs = 26, fat = 2 },
                new { foodName = "Banana", quantity = "1 unidade", calories = 90, protein = 1, carbs = 23, fat = 0 },
            }},
            new { mealType = 2, options = new[]
            {
                new { foodName = veg ? "Grão-de-bico cozido" : "Frango grelhado", quantity = "200g", calories = veg ? 240 : 220, protein = veg ? 14 : 42, carbs = veg ? 40 : 0, fat = veg ? 4 : 5 },
                new { foodName = "Arroz integral", quantity = "6 colheres de sopa", calories = 210, protein = 4, carbs = 45, fat = 1 },
                new { foodName = "Feijão cozido", quantity = "1 concha", calories = 140, protein = 9, carbs = 25, fat = 1 },
            }},
            new { mealType = 3, options = new[]
            {
                new { foodName = "Batata doce", quantity = "150g", calories = 135, protein = 3, carbs = 31, fat = 0 },
                new { foodName = veg ? "Iogurte grego" : "Frango grelhado", quantity = veg ? "200g" : "100g", calories = veg ? 120 : 110, protein = veg ? 20 : 21, carbs = veg ? 7 : 0, fat = veg ? 0 : 2 },
            }},
            new { mealType = 4, options = new[]
            {
                new { foodName = veg ? "Tofu grelhado" : "Carne vermelha magra", quantity = "200g", calories = veg ? 160 : 260, protein = veg ? 21 : 42, carbs = veg ? 4 : 0, fat = veg ? 8 : 10 },
                new { foodName = "Arroz integral", quantity = "4 colheres de sopa", calories = 140, protein = 3, carbs = 30, fat = 1 },
                new { foodName = "Legumes salteados", quantity = "100g", calories = 60, protein = 2, carbs = 12, fat = 1 },
            }},
            new { mealType = 5, options = new[]
            {
                new { foodName = "Queijo cottage", quantity = "200g", calories = 140, protein = 24, carbs = 8, fat = 2 },
                new { foodName = "Castanhas", quantity = "20g", calories = 130, protein = 3, carbs = 3, fat = 12 },
            }},
        };
        return ("Plano para Ganho de Massa", JsonSerializer.Serialize(meals), 2185);
    }

    private static (string name, string mealsJson, int calories) BalancedDietJson(bool veg)
    {
        var meals = new[]
        {
            new { mealType = 1, options = new[]
            {
                new { foodName = "Ovos mexidos", quantity = "2 unidades", calories = 140, protein = 12, carbs = 2, fat = 9 },
                new { foodName = "Pão integral", quantity = "2 fatias", calories = 140, protein = 6, carbs = 26, fat = 2 },
                new { foodName = "Fruta da estação", quantity = "1 unidade média", calories = 70, protein = 1, carbs = 18, fat = 0 },
            }},
            new { mealType = 2, options = new[]
            {
                new { foodName = veg ? "Feijão cozido" : "Frango grelhado", quantity = veg ? "2 conchas" : "180g", calories = veg ? 280 : 200, protein = veg ? 18 : 38, carbs = veg ? 50 : 0, fat = veg ? 2 : 4 },
                new { foodName = "Arroz integral", quantity = "5 colheres de sopa", calories = 175, protein = 4, carbs = 37, fat = 1 },
                new { foodName = "Salada variada", quantity = "à vontade", calories = 40, protein = 2, carbs = 8, fat = 0 },
            }},
            new { mealType = 3, options = new[]
            {
                new { foodName = "Iogurte grego natural", quantity = "170g", calories = 100, protein = 17, carbs = 6, fat = 0 },
                new { foodName = "Castanhas", quantity = "30g", calories = 195, protein = 4, carbs = 4, fat = 19 },
            }},
            new { mealType = 4, options = new[]
            {
                new { foodName = veg ? "Omelete de legumes" : "Omelete", quantity = "3 ovos", calories = 210, protein = 18, carbs = veg ? 8 : 3, fat = 14 },
                new { foodName = "Batata doce", quantity = "100g", calories = 90, protein = 2, carbs = 21, fat = 0 },
                new { foodName = "Legumes no vapor", quantity = "100g", calories = 50, protein = 3, carbs = 10, fat = 0 },
            }},
        };
        return ("Plano Alimentar Equilibrado", JsonSerializer.Serialize(meals), 1490);
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

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static string GetFirstName(string? fullName) =>
        (fullName ?? "aluno").Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

    private static bool HasAny(string source, params string[] terms) =>
        terms.Any(t => source.Contains(t, StringComparison.OrdinalIgnoreCase));

    private static bool HasAll(string source, params string[] terms) =>
        terms.All(t => source.Contains(t, StringComparison.OrdinalIgnoreCase));
}
