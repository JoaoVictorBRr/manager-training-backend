using System.Linq.Expressions;
using FluentValidation.Results;
using Zyntra.Domain.Dtos.EvolutionPhotoDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class EvolutionPhotoService(IEvolutionPhotoRepository repo) : IEvolutionPhotoService
{
    public async Task<IEnumerable<EvolutionPhotoResponseDto>> GetByStudentAsync(long studentId)
    {
        var photos = await repo.GetByStudentAsync(studentId);
        return photos.Select(p => new EvolutionPhotoResponseDto
        {
            Id = p.Id,
            ImageUrl = p.ImagePath,
            TakenAt = p.TakenAt,
            Notes = p.Notes,
        });
    }

    public async Task<EvolutionPhotoResponseDto> AddPhotoAsync(long studentId, string imagePath, string? notes)
    {
        var photo = new EvolutionPhoto
        {
            StudentId = studentId,
            ImagePath = imagePath,
            TakenAt = DateTime.Now,
            Notes = notes,
        };
        await repo.AddAsync(photo);

        return new EvolutionPhotoResponseDto
        {
            Id = photo.Id,
            ImageUrl = imagePath,
            TakenAt = photo.TakenAt,
            Notes = photo.Notes,
        };
    }

    // IServiceBase boilerplate
    public Task<EvolutionPhoto> AddAsync(EvolutionPhoto entity) => repo.AddAsync(entity);
    public Task AddRangeAsync(IList<EvolutionPhoto> entity) => repo.AddRangeAsync(entity);
    public Task<IList<EvolutionPhoto>> AddRangeListAsync(IList<EvolutionPhoto> entities) => repo.AddRangeListAsync(entities);
    public Task<EvolutionPhoto> UpdateAsync(EvolutionPhoto entity) => repo.UpdateAsync(entity);
    public Task UpdateRangeAsync(IEnumerable<EvolutionPhoto> entity) => repo.UpdateRangeAsync(entity);
    public Task<EvolutionPhoto> DeleteAsync(EvolutionPhoto entity) => repo.DeleteAsync(entity);
    public Task<EvolutionPhoto> DeleteAsync(long Id) => repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<EvolutionPhoto> entities) => repo.DeleteRangeAsync(entities);
    public Task<EvolutionPhoto> GetByIdAsync(long Id) => repo.GetByIdAsync(Id);
    public Task<IEnumerable<EvolutionPhoto>> GetAllAsync(Expression<Func<EvolutionPhoto, bool>> predicate) => repo.GetAllAsync(predicate);
    public Task<EvolutionPhoto> GetAsync(Expression<Func<EvolutionPhoto, bool>> predicate) => repo.GetAsync(predicate);
    public Task<List<ValidationFailure>> Validate(EvolutionPhoto entity) => Task.FromResult(new List<ValidationFailure>());
}
