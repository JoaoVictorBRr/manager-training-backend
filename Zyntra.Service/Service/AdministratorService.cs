using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class AdministratorService(IAdministratorRepository administratorRepository) : BaseService<Administrator>(administratorRepository), IAdministratorService
{
}
