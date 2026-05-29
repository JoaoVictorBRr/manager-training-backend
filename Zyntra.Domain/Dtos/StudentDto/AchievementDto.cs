namespace Zyntra.Domain.Dtos.StudentDto;

public class AchievementDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool Unlocked { get; set; }
    public string? UnlockedLabel { get; set; }
}
