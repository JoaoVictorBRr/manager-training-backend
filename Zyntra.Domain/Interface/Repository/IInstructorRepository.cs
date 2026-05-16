using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IInstructorRepository : IRepositoryBase<Instructor>
{
    Task<PagedListDto<Instructor>> FilterAllInstructors(InstructorRequestListDto filter);
    Task<Instructor> GetByUserIdAsync(long userId);
}
