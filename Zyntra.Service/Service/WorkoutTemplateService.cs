using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class WorkoutTemplateService(
    IWorkoutSheetRepository workoutSheetRepository,
    IExerciseRepository exerciseRepository,
    IInstructorRepository instructorRepository) : IWorkoutTemplateService
{
    private record ExerciseTemplate(
        string Name, string MuscleGroup, int Sets, string Repetitions,
        string? RestTime = "60s", string? Description = null, string? Difficulty = "medium",
        ExerciseType ExerciseType = ExerciseType.WeightTraining);

    private static readonly Dictionary<string, ExerciseTemplate[]> ExerciseLibrary = new()
    {
        ["Peito"] =
        [
            new("Supino Reto com Barra", "Peito", 4, "8-12", "90s", "Deite no banco, desça a barra até o peito e empurre de volta.", "medium"),
            new("Supino Inclinado com Halteres", "Peito", 3, "10-12", "60s", "Banco inclinado a 45°. Desça os halteres até a altura do peito.", "medium"),
            new("Crucifixo com Halteres", "Peito", 3, "12-15", "60s", "Braços semiflexionados, abra e feche simulando um abraço.", "easy"),
            new("Flexão de Braço", "Peito", 3, "Máximo", "60s", "Apoio nas mãos e pontas dos pés. Desça o peito até quase tocar o chão.", "easy", ExerciseType.Bodyweight),
            new("Supino Declinado com Barra", "Peito", 3, "10-12", "90s", "Banco declinado. Foca na porção inferior do peitoral.", "medium"),
        ],
        ["Costas"] =
        [
            new("Puxada Frontal", "Costas", 4, "10-12", "60s", "Puxe a barra até a altura do queixo, cotovelos apontando para baixo.", "medium"),
            new("Remada Curvada com Barra", "Costas", 4, "8-10", "90s", "Tronco inclinado a 45°, puxe a barra até o abdômen.", "medium"),
            new("Remada Unilateral com Halter", "Costas", 3, "10-12", "60s", "Apoio em um banco, puxe o halter até a cintura.", "easy"),
            new("Pull-Up (Barra Fixa)", "Costas", 3, "Máximo", "90s", "Pegada pronada, suba até o queixo passar da barra.", "hard", ExerciseType.Bodyweight),
            new("Remada Baixa no Cabo", "Costas", 3, "12-15", "60s", "Sentado, puxe o cabo até o abdômen mantendo as costas retas.", "easy"),
        ],
        ["Pernas"] =
        [
            new("Agachamento Livre", "Pernas", 4, "8-12", "120s", "Pés na largura dos ombros, desça até a coxa ficar paralela ao chão.", "hard"),
            new("Leg Press 45°", "Pernas", 4, "10-15", "90s", "Pés na plataforma, empurre até quase estender os joelhos.", "medium"),
            new("Extensão de Perna", "Pernas", 3, "12-15", "60s", "Sentado na máquina, estenda as pernas completamente.", "easy"),
            new("Flexão de Perna (Cadeira Flexora)", "Pernas", 3, "12-15", "60s", "Deitado, curve as pernas até quase tocar os glúteos.", "easy"),
            new("Panturrilha em Pé", "Pernas", 4, "15-20", "45s", "Em pé, suba na ponta dos pés e desça controlado.", "easy"),
            new("Avanço com Halteres", "Pernas", 3, "10-12/perna", "60s", "Passo à frente, joelho quase toca o chão, retorne.", "medium"),
        ],
        ["Ombros"] =
        [
            new("Desenvolvimento com Halteres", "Ombros", 4, "10-12", "60s", "Sentado ou em pé, empurre os halteres acima da cabeça.", "medium"),
            new("Elevação Lateral", "Ombros", 3, "12-15", "45s", "Eleve os halteres lateralmente até a altura dos ombros.", "easy"),
            new("Elevação Frontal", "Ombros", 3, "12-15", "45s", "Eleve os halteres à frente até a altura dos olhos.", "easy"),
            new("Desenvolvimento Arnold", "Ombros", 3, "10-12", "60s", "Giro dos punhos de dentro para fora ao elevar.", "medium"),
            new("Encolhimento de Ombros", "Ombros", 3, "15-20", "45s", "Eleve os ombros em direção às orelhas com halteres nas mãos.", "easy"),
        ],
        ["Bíceps"] =
        [
            new("Rosca Direta com Barra", "Bíceps", 3, "10-12", "60s", "Em pé, dobre os cotovelos elevando a barra até os ombros.", "easy"),
            new("Rosca Alternada com Halteres", "Bíceps", 3, "10-12/braço", "60s", "Alterne os braços, supine o pulso no topo do movimento.", "easy"),
            new("Rosca Concentrada", "Bíceps", 3, "12-15", "45s", "Sentado, cotovelo apoiado na coxa, curl completo.", "easy"),
            new("Rosca Martelo", "Bíceps", 3, "10-12", "60s", "Pegada neutra, sem girar o pulso.", "easy"),
        ],
        ["Tríceps"] =
        [
            new("Tríceps Pulley (Cabo)", "Tríceps", 3, "12-15", "45s", "Em pé no cabo, empurre a barra para baixo mantendo cotovelos fixos.", "easy"),
            new("Tríceps Testa com Barra", "Tríceps", 3, "10-12", "60s", "Deitado, dobre os cotovelos abaixando a barra até a testa.", "medium"),
            new("Mergulho em Banco (Banco)", "Tríceps", 3, "Máximo", "60s", "Mãos apoiadas no banco atrás, dobre e estenda os cotovelos.", "easy", ExerciseType.Bodyweight),
            new("Tríceps Francês com Halter", "Tríceps", 3, "12-15", "60s", "Um halter acima da cabeça, dobre os cotovelos descendo-o atrás.", "easy"),
        ],
        ["Abdômen"] =
        [
            new("Abdominal Crunch", "Abdômen", 3, "20-25", "45s", "Deitado, eleve o tronco contraindo o abdômen.", "easy", ExerciseType.Bodyweight),
            new("Prancha (Plank)", "Abdômen", 3, "30-60s", "45s", "Apoio em antebraços e pés, mantenha o corpo reto.", "easy", ExerciseType.Bodyweight),
            new("Abdominal Bicicleta", "Abdômen", 3, "20/lado", "45s", "Cotovelo encontra o joelho oposto alternadamente.", "easy", ExerciseType.Bodyweight),
            new("Elevação de Pernas", "Abdômen", 3, "15-20", "45s", "Deitado, eleve as pernas mantendo-as retas.", "medium", ExerciseType.Bodyweight),
        ],
    };

    private static List<(string Day, string[] MuscleGroups)> GetSplit(int trainingDays) => trainingDays switch
    {
        1 => [("A", ["Peito", "Costas", "Pernas", "Ombros", "Bíceps", "Tríceps", "Abdômen"])],
        2 =>
        [
            ("A", ["Peito", "Ombros", "Tríceps", "Abdômen"]),
            ("B", ["Costas", "Bíceps", "Pernas"]),
        ],
        3 =>
        [
            ("A", ["Peito", "Tríceps", "Ombros"]),
            ("B", ["Costas", "Bíceps"]),
            ("C", ["Pernas", "Abdômen"]),
        ],
        4 =>
        [
            ("A", ["Peito", "Tríceps"]),
            ("B", ["Costas", "Bíceps"]),
            ("C", ["Pernas"]),
            ("D", ["Ombros", "Abdômen"]),
        ],
        5 =>
        [
            ("A", ["Peito"]),
            ("B", ["Costas"]),
            ("C", ["Pernas"]),
            ("D", ["Ombros"]),
            ("E", ["Bíceps", "Tríceps", "Abdômen"]),
        ],
        6 =>
        [
            ("A", ["Peito", "Tríceps"]),
            ("B", ["Costas", "Bíceps"]),
            ("C", ["Pernas"]),
            ("D", ["Ombros", "Abdômen"]),
            ("E", ["Peito", "Tríceps"]),
            ("F", ["Costas", "Bíceps"]),
        ],
        _ =>
        [
            ("A", ["Peito", "Tríceps"]),
            ("B", ["Costas", "Bíceps"]),
            ("C", ["Pernas"]),
            ("D", ["Ombros", "Abdômen"]),
            ("E", ["Peito", "Tríceps"]),
            ("F", ["Costas", "Bíceps"]),
            ("G", ["Pernas"]),
        ],
    };

    public async Task<WorkoutSheet> GenerateWorkoutAsync(long studentId, int trainingDays)
    {
        // Deactivate current active workout
        var current = await workoutSheetRepository.GetActiveByStudentAsync(studentId);
        if (current != null)
        {
            current.IsActive = false;
            await workoutSheetRepository.UpdateAsync(current);
        }

        // Use existing instructor ID or fallback to first available
        long instructorId = current?.InstructorId ?? 0;
        if (instructorId == 0)
        {
            var firstInstructor = (await instructorRepository.GetAllAsync(_ => true)).FirstOrDefault();
            instructorId = firstInstructor?.Id ?? 0;
        }

        var clampedDays = Math.Clamp(trainingDays, 1, 7);
        var split = GetSplit(clampedDays);
        var totalGroups = split.SelectMany(s => s.MuscleGroups).Distinct().Count();
        var notes = $"Treino gerado automaticamente — {clampedDays} dias/semana";

        var sheet = await workoutSheetRepository.AddAsync(new WorkoutSheet
        {
            StudentId = studentId,
            InstructorId = instructorId,
            StartDate = DateTime.UtcNow,
            IsActive = true,
            Notes = notes,
        });

        foreach (var (day, muscleGroups) in split)
        {
            foreach (var group in muscleGroups)
            {
                if (!ExerciseLibrary.TryGetValue(group, out var templates)) continue;

                // Take up to 3-4 exercises per group per day
                var exercisesPerGroup = muscleGroups.Length == 1 ? 5 : 3;
                foreach (var t in templates.Take(exercisesPerGroup))
                {
                    await exerciseRepository.AddAsync(new Exercise
                    {
                        WorkoutSheetId = sheet.Id,
                        Name = t.Name,
                        MuscleGroup = t.MuscleGroup,
                        Sets = t.Sets,
                        Repetitions = t.Repetitions,
                        RestTime = t.RestTime,
                        Description = t.Description ?? string.Empty,
                        Difficulty = t.Difficulty,
                        WorkoutDay = day,
                        ExerciseType = t.ExerciseType,
                        IsDropset = false,
                        ToFailure = false,
                    });
                }
            }
        }

        // Reload with navigation properties
        return await workoutSheetRepository.GetActiveByStudentAsync(studentId);
    }
}
