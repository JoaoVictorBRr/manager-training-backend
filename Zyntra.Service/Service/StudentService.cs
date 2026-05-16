using FluentValidation.Results;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class StudentService(IStudentRepository studentRepository) : BaseService<Student>(studentRepository), IStudentService
{
    private readonly IStudentRepository _studentRepository = studentRepository;

    public Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter)
        => _studentRepository.FilterAllStudents(filter);

    public Task<List<ValidationFailure>> Validate(Student entity)
        => Task.FromResult(new List<ValidationFailure>());
}
