using System.ComponentModel;

namespace Zyntra.Domain.Enum;
public enum Role
{
    [Description("Administrador")]
    Administrator = 1,
    [Description("Instrutor")]
    Instructor = 2,
    [Description("Aluno")]
    Student = 3,
}