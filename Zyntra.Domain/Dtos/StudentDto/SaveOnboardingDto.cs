namespace Zyntra.Domain.Dtos.StudentDto;

public class SaveOnboardingDto
{
    public string? Objective { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public decimal? TargetWeight { get; set; }
    public int? TargetDeadline { get; set; }
    public int? TrainingDays { get; set; }
    public string? TrainingDuration { get; set; }
    public string? PreferredTime { get; set; }
    public string? Environment { get; set; }
    public List<string>? Equipment { get; set; }
    public List<string>? Injuries { get; set; }
    public List<string>? Restrictions { get; set; }
    public List<string>? DietRestrictions { get; set; }
    public int? MealsPerDay { get; set; }
    public decimal? WaterIntake { get; set; }
    public List<string>? FavoriteFoods { get; set; }
    public List<string>? Motivation { get; set; }
    public List<string>? QuitReasons { get; set; }
    public string? OnboardingDataJson { get; set; }
}
