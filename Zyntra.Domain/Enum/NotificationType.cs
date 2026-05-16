using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum NotificationType
{
    [Description("Lembrete de Aula")]
    ClassReminder = 1,
    [Description("Lembrete de Treino")]
    WorkoutReminder = 2,
    [Description("Pagamento Pendente")]
    PaymentDue = 3,
    [Description("Mensagem")]
    Message = 4,
    [Description("Promoção")]
    Promotion = 5,
}
