using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum ScheduleStatus
{
    [Description("Confirmado")]
    Confirmed = 1,
    [Description("Cancelado")]
    Cancelled = 2,
    [Description("Lista de Espera")]
    WaitList = 3,
}
