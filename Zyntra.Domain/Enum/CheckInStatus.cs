using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum CheckInStatus
{
    [Description("Pendente")]
    Pending = 1,
    [Description("Aprovado")]
    Approved = 2,
    [Description("Bloqueado")]
    Blocked = 3,
}
