using System.Linq.Expressions;
using FluentValidation.Results;

namespace Zyntra.Domain.Interface.Service.Base;
public interface IServiceBase<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task AddRangeAsync(IList<TEntity> entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task UpdateRangeAsync(IEnumerable<TEntity> entity);
    Task<TEntity> GetByIdAsync(long Id);
    Task<TEntity> DeleteAsync(TEntity entity);
    Task<TEntity> DeleteAsync(long Id);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task DeleteRangeAsync(IList<TEntity> entities);
    Task<IList<TEntity>> AddRangeListAsync(IList<TEntity> entities);
    Task<List<ValidationFailure>> Validate(TEntity entity);
}
