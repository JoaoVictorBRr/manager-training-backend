using FluentValidation.Results;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class InstructorService(IInstructorRepository instructorRepository) : BaseService<Instructor>(instructorRepository), IInstructorService
{
    private readonly IInstructorRepository _instructorRepository = instructorRepository;

    public Task<PagedListDto<Instructor>> FilterAllInstructors(InstructorRequestListDto filter)
        => _instructorRepository.FilterAllInstructors(filter);

    public Task<List<ValidationFailure>> Validate(Instructor entity)
        => Task.FromResult(new List<ValidationFailure>());
}
