using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using System.Text.Json;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class StudentService(
    IStudentRepository repo,
    IValidator<Student> validator,
    IPhysicalAssessmentRepository assessmentRepo,
    ICheckInRepository checkInRepo,
    IStudentAchievementRepository achievementRepo) : IStudentService
{
    private readonly IStudentRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Student> _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IPhysicalAssessmentRepository _assessmentRepo = assessmentRepo ?? throw new ArgumentNullException(nameof(assessmentRepo));
    private readonly ICheckInRepository _checkInRepo = checkInRepo ?? throw new ArgumentNullException(nameof(checkInRepo));
    private readonly IStudentAchievementRepository _achievementRepo = achievementRepo ?? throw new ArgumentNullException(nameof(achievementRepo));

    public async Task<Student> AddAsync(Student entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Student> UpdateAsync(Student entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Student> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Student>> AddRangeListAsync(IList<Student> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Student> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Student> DeleteAsync(Student entity) => _repo.DeleteAsync(entity);
    public Task<Student> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Student> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Student> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Student> GetAsync(Expression<Func<Student, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter)
        => _repo.FilterAllStudents(filter);

    public async Task<Student?> GetByUserIdAsync(long userId)
        => await _repo.GetByUserIdAsync(userId);

    public async Task<Student> SaveOnboardingAsync(long userId, SaveOnboardingDto dto)
    {
        var student = await _repo.GetByUserIdAsync(userId)
            ?? throw new InvalidOperationException("Perfil de aluno não encontrado.");

        student.Objective = dto.Objective;
        student.OnboardingDataJson = dto.OnboardingDataJson ?? JsonSerializer.Serialize(dto);
        student.OnboardingCompleted = true;

        await _repo.UpdateAsync(student);

        if (dto.Weight.HasValue && dto.Height.HasValue)
        {
            var heightM = dto.Height.Value > 10 ? dto.Height.Value / 100m : dto.Height.Value;
            var bmi = dto.Weight.Value / (heightM * heightM);

            await _assessmentRepo.AddAsync(new PhysicalAssessment
            {
                StudentId = student.Id,
                AssessmentDate = DateTime.UtcNow,
                Weight = dto.Weight.Value,
                Height = heightM,
                Bmi = Math.Round(bmi, 2),
                Notes = "Avaliação inicial — dados do onboarding"
            });
        }

        return student;
    }

    public async Task<StudentStatsDto> GetStatsAsync(Student student)
    {
        var checkIns = (await _checkInRepo.GetAllAsync(c => c.StudentId == student.Id))
            .OrderByDescending(c => c.DateTime)
            .ToList();

        // Streak
        var streak = 0;
        var expected = DateOnly.FromDateTime(DateTime.Today);
        foreach (var checkIn in checkIns)
        {
            var day = DateOnly.FromDateTime(checkIn.DateTime);
            if (day == expected) { streak++; expected = expected.AddDays(-1); }
            else if (day < expected) break;
        }

        // Weekly check-ins (Monday-based: 0=Mon, 6=Sun)
        var startOfWeek = DateTime.Today.AddDays(-(((int)DateTime.Today.DayOfWeek + 6) % 7));
        var thisWeek = checkIns.Where(c => c.DateTime.Date >= startOfWeek.Date).ToList();
        var weeklyDays = thisWeek
            .Select(c => ((int)c.DateTime.DayOfWeek + 6) % 7)
            .Distinct()
            .ToArray();

        // XP and level
        var xp = checkIns.Count * 100;

        // TDEE via Harris-Benedict from onboarding data
        int calorieGoal = 2000;
        SaveOnboardingDto? onboarding = null;
        if (!string.IsNullOrEmpty(student.OnboardingDataJson))
        {
            try { onboarding = JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); }
            catch { /* ignore parse errors */ }
        }

        if (onboarding?.Weight > 0 && onboarding?.Height > 0 && onboarding?.Age > 0)
        {
            var heightCm = onboarding.Height!.Value > 10 ? (double)onboarding.Height.Value : (double)onboarding.Height.Value * 100;
            double bmr = onboarding.Gender?.ToLower() == "female"
                ? 447.593 + (9.247 * (double)onboarding.Weight!.Value) + (3.098 * heightCm) - (4.330 * onboarding.Age!.Value)
                : 88.362 + (13.397 * (double)onboarding.Weight!.Value) + (4.799 * heightCm) - (5.677 * onboarding.Age!.Value);

            double activityMultiplier = (onboarding.TrainingDays ?? 3) switch
            {
                <= 2 => 1.375,
                <= 4 => 1.55,
                <= 6 => 1.725,
                _ => 1.9
            };
            calorieGoal = (int)(bmr * activityMultiplier);
            if (onboarding.Objective == "lose_weight") calorieGoal -= 400;
            else if (onboarding.Objective == "gain_muscle") calorieGoal += 300;
            calorieGoal = Math.Max(1200, calorieGoal);
        }

        return new StudentStatsDto
        {
            CurrentStreak = streak,
            TotalCheckIns = checkIns.Count,
            Xp = xp,
            Level = Math.Max(1, xp / 500),
            WeeklyCheckInDays = weeklyDays,
            TotalCheckInsThisWeek = thisWeek.Count,
            CalorieGoal = calorieGoal,
            ProteinGoal = (int)(calorieGoal * 0.30 / 4),
            CarbGoal = (int)(calorieGoal * 0.45 / 4),
            FatGoal = (int)(calorieGoal * 0.25 / 9),
        };
    }

    public async Task<IEnumerable<AchievementDto>> GetAchievementsAsync(long studentId)
    {
        var student = await _repo.GetByUserIdAsync(studentId) ?? new Student { Id = studentId };
        // Use studentId directly for check-ins
        var checkIns = await _checkInRepo.GetAllAsync(c => c.StudentId == studentId);
        var total = checkIns.Count();

        // Recompute streak
        var ordered = checkIns.OrderByDescending(c => c.DateTime).ToList();
        var streak = 0;
        var expected = DateOnly.FromDateTime(DateTime.Today);
        foreach (var c in ordered)
        {
            var day = DateOnly.FromDateTime(c.DateTime);
            if (day == expected) { streak++; expected = expected.AddDays(-1); }
            else if (day < expected) break;
        }

        var milestones = new[]
        {
            (id: "first",  title: "Primeiro Treino",   icon: "zap",    checkIns: 1,   streakNeeded: 0),
            (id: "ci5",    title: "Semana de Fogo",    icon: "flame",  checkIns: 5,   streakNeeded: 0),
            (id: "ci20",   title: "1 Mês de Treinos",  icon: "star",   checkIns: 20,  streakNeeded: 0),
            (id: "ci50",   title: "50 Treinos",        icon: "trophy", checkIns: 50,  streakNeeded: 0),
            (id: "ci100",  title: "100 Treinos",       icon: "target", checkIns: 100, streakNeeded: 0),
            (id: "ci200",  title: "Veterano",          icon: "trophy", checkIns: 200, streakNeeded: 0),
        };

        var persisted = (await _achievementRepo.GetByStudentAsync(studentId))
            .ToDictionary(a => a.AchievementKey);

        var result = new List<AchievementDto>();
        foreach (var m in milestones)
        {
            bool unlocked = (m.checkIns == 0 || total >= m.checkIns)
                         && (m.streakNeeded == 0 || streak >= m.streakNeeded);

            if (unlocked && !persisted.ContainsKey(m.id))
            {
                await _achievementRepo.AddAsync(new StudentAchievement
                {
                    StudentId = studentId,
                    AchievementKey = m.id,
                    Title = m.title,
                    Icon = m.icon,
                    UnlockedAt = DateTime.UtcNow,
                });
            }

            string? unlockedLabel = null;
            if (unlocked)
            {
                var unlockedAt = persisted.TryGetValue(m.id, out var saved) ? saved.UnlockedAt : DateTime.UtcNow;
                unlockedLabel = $"Conquistado em {unlockedAt:dd/MM/yyyy}";
            }

            result.Add(new AchievementDto
            {
                Id = m.id,
                Title = m.title,
                Icon = m.icon,
                Unlocked = unlocked,
                UnlockedLabel = unlockedLabel,
            });
        }
        return result;
    }

    public async Task<IEnumerable<WeeklyCheckInDto>> GetWeeklyCheckInHistoryAsync(long studentId, int weeksBack = 5)
    {
        var cutoff = DateTime.Today.AddDays(-weeksBack * 7);
        var checkIns = (await _checkInRepo.GetAllAsync(c => c.StudentId == studentId && c.DateTime >= cutoff))
            .ToList();

        var result = new List<WeeklyCheckInDto>();
        for (int i = weeksBack - 1; i >= 0; i--)
        {
            var weekStart = DateTime.Today.AddDays(-(((int)DateTime.Today.DayOfWeek + 6) % 7) - i * 7);
            var weekEnd = weekStart.AddDays(7);
            var count = checkIns.Count(c => c.DateTime.Date >= weekStart.Date && c.DateTime.Date < weekEnd.Date);
            result.Add(new WeeklyCheckInDto { Week = $"Sem {weeksBack - i}", Count = count });
        }
        return result;
    }

    public async Task<List<ValidationFailure>> Validate(Student entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
