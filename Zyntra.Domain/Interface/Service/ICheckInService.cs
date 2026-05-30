using Zyntra.Domain.Dtos.CheckInDto;

namespace Zyntra.Domain.Interface.Service;

public interface ICheckInService
{
    Task<CheckInHistoryDto> CheckInAsync(long studentId, string workoutDay);
    Task<IEnumerable<CheckInHistoryDto>> GetHistoryAsync(long studentId);
}
