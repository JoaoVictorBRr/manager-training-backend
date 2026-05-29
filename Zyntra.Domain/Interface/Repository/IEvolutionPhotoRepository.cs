using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IEvolutionPhotoRepository : IRepositoryBase<EvolutionPhoto>
{
    Task<IEnumerable<EvolutionPhoto>> GetByStudentAsync(long studentId);
}
