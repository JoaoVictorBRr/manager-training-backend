using FluentValidation.Results;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IStudentService : IServiceBase<Student>
{
    Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter);
    Task<Student?> GetByUserIdAsync(long userId);
    Task<List<ValidationFailure>> Validate(Student entity);
    Task<Student> SaveOnboardingAsync(long userId, SaveOnboardingDto dto);
    Task<StudentStatsDto> GetStatsAsync(Student student);
    Task<IEnumerable<AchievementDto>> GetAchievementsAsync(long studentId);
    Task<IEnumerable<WeeklyCheckInDto>> GetWeeklyCheckInHistoryAsync(long studentId, int weeksBack = 5);
}
