using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface ICheckInService : IServiceBase<CheckIn>
{
    Task<CheckIn> CheckInAsync(CheckInRequestDto request);
    Task<CheckIn> CheckInPartnerAsync(CheckInPartnerRequestDto request);
}
