using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class DietMealOptionRepository(ZyntraContext zyntraContext) : BaseRepository<DietMealOption>(zyntraContext), IDietMealOptionRepository
{
}
