using FluentValidation.Results;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IInstructorService : IServiceBase<Instructor>
{
    Task<PagedListDto<Instructor>> FilterAllInstructors(InstructorRequestListDto filter);
    Task<List<ValidationFailure>> Validate(Instructor entity);
}
