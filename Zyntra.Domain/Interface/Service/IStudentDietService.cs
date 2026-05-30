using Zyntra.Domain.Dtos.DietDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IStudentDietService : IServiceBase<StudentDiet>
{
    Task<StudentDietResponseDto?> GetActiveDietAsync(long studentId);
    Task<StudentDietResponseDto> CreateDietAsync(long studentId, CreateDietDto dto);
    Task<StudentDietResponseDto> GenerateDefaultDietAsync(long studentId, string? objective, List<string>? dietRestrictions);
    Task CompleteMealAsync(long dietMealId);
    Task AddMealPhotoAsync(long dietMealOptionId, string imagePath);
}
