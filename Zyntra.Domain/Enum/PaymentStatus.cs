using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum PaymentStatus
{
    [Description("Pendente")]
    Pending = 1,
    [Description("Pago")]
    Paid = 2,
    [Description("Vencido")]
    Overdue = 3,
    [Description("Cancelado")]
    Cancelled = 4,
}
