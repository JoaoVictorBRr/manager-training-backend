using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Student : EntityBase
{
    public long UserId { get; set; }
    public DateTime BirthDate { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public string? Objective { get; set; }
    public string? OnboardingDataJson { get; set; }
    public bool OnboardingCompleted { get; set; } = false;

    public User User { get; set; }
}
