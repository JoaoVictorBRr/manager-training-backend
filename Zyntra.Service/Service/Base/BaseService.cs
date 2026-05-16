using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using Zyntra.Domain.Interface.Repository.Base;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Service.Service.Base;

public abstract class BaseService<TEntity>(IRepositoryBase<TEntity> repository) : IServiceBase<TEntity> where TEntity : class
{
    protected readonly IRepositoryBase<TEntity> _repository = repository;

    protected virtual Task<List<ValidationFailure>> Validate(TEntity entity)
        => Task.FromResult(new List<ValidationFailure>());

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors.Count > 0)
            throw new ValidationException(errors);
        return await _repository.AddAsync(entity);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors.Count > 0)
            throw new ValidationException(errors);
        return await _repository.UpdateAsync(entity);
    }

    public virtual Task AddRangeAsync(IList<TEntity> entity) => _repository.AddRangeAsync(entity);
    public virtual Task<IList<TEntity>> AddRangeListAsync(IList<TEntity> entities) => _repository.AddRangeListAsync(entities);
    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entity) => _repository.UpdateRangeAsync(entity);
    public virtual Task<TEntity> DeleteAsync(TEntity entity) => _repository.DeleteAsync(entity);
    public virtual Task<TEntity> DeleteAsync(long Id) => _repository.DeleteAsync(Id);
    public virtual Task DeleteRangeAsync(IList<TEntity> entities) => _repository.DeleteRangeAsync(entities);
    public virtual Task<TEntity> GetByIdAsync(long Id) => _repository.GetByIdAsync(Id);
    public virtual Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) => _repository.GetAllAsync(predicate);
    public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate) => _repository.GetAsync(predicate);
}
