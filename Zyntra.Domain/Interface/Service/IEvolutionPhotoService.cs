using Zyntra.Domain.Dtos.EvolutionPhotoDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IEvolutionPhotoService : IServiceBase<EvolutionPhoto>
{
    Task<IEnumerable<EvolutionPhotoResponseDto>> GetByStudentAsync(long studentId);
    Task<EvolutionPhotoResponseDto> AddPhotoAsync(long studentId, string imagePath, string? notes);
}
