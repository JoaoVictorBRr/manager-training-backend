using FluentValidation.Results;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IStudentService : IServiceBase<Student>
{
    Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter);
    Task<List<ValidationFailure>> Validate(Student entity);
}
