using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum CheckInType
{
    [Description("App")]
    App = 1,
    [Description("Gympass")]
    Gympass = 2,
    [Description("TotalPass")]
    TotalPass = 3,
}
