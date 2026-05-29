namespace Zyntra.Domain.Dtos.StudentDto;

public class StudentStatsDto
{
    public int CurrentStreak { get; set; }
    public int TotalCheckIns { get; set; }
    public int Xp { get; set; }
    public int Level { get; set; }
    public int[] WeeklyCheckInDays { get; set; } = [];
    public int TotalCheckInsThisWeek { get; set; }
    public int CalorieGoal { get; set; }
    public int ProteinGoal { get; set; }
    public int CarbGoal { get; set; }
    public int FatGoal { get; set; }
}
