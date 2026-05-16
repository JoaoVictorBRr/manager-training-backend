using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IWaitListRepository : IRepositoryBase<WaitList>
{
    Task<WaitList> GetFirstInLineAsync(long classId);
    Task<WaitList> GetByStudentAndClassAsync(long studentId, long classId);
}
