using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IStudentRepository : IRepositoryBase<Student>
{
    Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter);
    Task<Student> GetByUserIdAsync(long userId);
}
