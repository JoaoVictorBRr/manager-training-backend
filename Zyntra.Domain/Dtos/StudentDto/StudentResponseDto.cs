using System.Text.Json.Serialization;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.StudentDto;

public class StudentResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string CellphoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public Situation Situation { get; set; }

    [JsonPropertyName("membershipStatus")]
    public string MembershipStatus { get; set; }

    [JsonPropertyName("onboardingCompleted")]
    public bool OnboardingCompleted { get; set; }

    [JsonPropertyName("onboardingData")]
    public SaveOnboardingDto? OnboardingData { get; set; }

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("subscriptionPlan")]
    public int SubscriptionPlan { get; set; }
}
