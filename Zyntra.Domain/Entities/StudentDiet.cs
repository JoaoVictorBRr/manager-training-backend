using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class StudentDiet : EntityBase
{
    public long StudentId { get; set; }
    public string Name { get; set; } = "Plano Alimentar";
    public bool IsActive { get; set; } = true;
    public DateTime? GeneratedAt { get; set; }

    public Student Student { get; set; }
    public ICollection<DietMeal> Meals { get; set; } = [];
}
