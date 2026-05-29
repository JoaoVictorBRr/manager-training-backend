using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IStudentDietRepository : IRepositoryBase<StudentDiet>
{
    Task<StudentDiet?> GetActiveDietByStudentAsync(long studentId);
}
