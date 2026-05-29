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
    public string? AvatarUrl { get; set; }
    public int SubscriptionPlan { get; set; } = 3; // 1=Básico, 2=Padrão, 3=Pro

    public User User { get; set; }
}
