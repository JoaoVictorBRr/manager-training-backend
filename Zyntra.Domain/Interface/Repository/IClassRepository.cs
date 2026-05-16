using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IClassRepository : IRepositoryBase<Class>
{
    Task<PagedListDto<Class>> FilterAllClasses(ClassRequestListDto filter);
    Task<IEnumerable<Class>> GetAvailableClassesAsync();
}
