using System.ComponentModel;

namespace Zyntra.Domain.Enum;

public enum MealType
{
    [Description("Café da Manhã")]
    Breakfast = 1,
    [Description("Almoço")]
    Lunch = 2,
    [Description("Café da Tarde")]
    AfternoonSnack = 3,
    [Description("Janta")]
    Dinner = 4,
    [Description("Ceia")]
    Supper = 5,
}
