namespace Zyntra.Domain.Dtos.PaymentDto;

public class SubscribePlanDto
{
    public int PlanId { get; set; }
    public string CardHolder { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
}
