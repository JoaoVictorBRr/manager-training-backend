using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class CheckInService(ICheckInRepository repo) : ICheckInService
{
    public async Task<CheckInHistoryDto> CheckInAsync(long studentId, string workoutDay)
    {
        var checkIn = new CheckIn
        {
            StudentId = studentId,
            DateTime = DateTime.UtcNow,
            Unit = "Principal",
            AccessType = CheckInType.App,
            ValidationStatus = CheckInStatus.Approved,
            WorkoutDayPerformed = workoutDay,
            UserIdCreated = studentId,
        };

        await repo.AddAsync(checkIn);

        return new CheckInHistoryDto
        {
            Id = checkIn.Id,
            DateTime = checkIn.DateTime,
            WorkoutDayPerformed = checkIn.WorkoutDayPerformed,
        };
    }

    public async Task<IEnumerable<CheckInHistoryDto>> GetHistoryAsync(long studentId)
    {
        var items = await repo.GetAllAsync(c => c.StudentId == studentId);
        return items
            .OrderByDescending(c => c.DateTime)
            .Select(c => new CheckInHistoryDto
            {
                Id = c.Id,
                DateTime = c.DateTime,
                WorkoutDayPerformed = c.WorkoutDayPerformed,
            });
    }
}
