using FluentValidation.Results;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IClassService : IServiceBase<Class>
{
    Task<PagedListDto<Class>> FilterAllClasses(ClassRequestListDto filter);
    Task<IEnumerable<Class>> GetAvailableClassesAsync();
    Task<List<ValidationFailure>> Validate(Class entity);
}
