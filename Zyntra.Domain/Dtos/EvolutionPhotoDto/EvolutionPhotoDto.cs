namespace Zyntra.Domain.Dtos.EvolutionPhotoDto;

public class EvolutionPhotoResponseDto
{
    public long Id { get; set; }
    public string ImageUrl { get; set; }
    public DateTime TakenAt { get; set; }
    public string? Notes { get; set; }
}
