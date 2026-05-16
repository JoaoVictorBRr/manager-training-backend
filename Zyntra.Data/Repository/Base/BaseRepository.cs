using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zyntra.Data.Context;
using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Data.Repository.Base;
public abstract class BaseRepository<TEntity>(ZyntraContext zyntraContext) : IRepositoryBase<TEntity> where TEntity : class
{
    protected readonly ZyntraContext _zyntraContext = zyntraContext;
    protected readonly DbSet<TEntity> DbSet = zyntraContext.Set<TEntity>();

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        try
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            if (entity is EntityBase entityBase)
                entityBase.DateCreated = DateTime.Now;

            await _zyntraContext.Set<TEntity>().AddAsync(entity);
            await _zyntraContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public virtual async Task AddRangeAsync(IList<TEntity> entities)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        var insertEntities = new List<TEntity>();
        foreach (var entity in entities)
        {
            if (entity is EntityBase entityBase)
            {
                entityBase.Situation = Situation.Active;
                entityBase.DateCreated = DateTime.Now;
            }
            insertEntities.Add(entity);
        }

        await DbSet.AddRangeAsync(insertEntities);
        await _zyntraContext.SaveChangesAsync();
    }

    public virtual async Task<IList<TEntity>> AddRangeListAsync(IList<TEntity> entities)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        var insertEntities = new List<TEntity>();
        foreach (var entity in entities)
        {
            if (entity is EntityBase entityBase)
            {
                entityBase.Situation = Situation.Active;
                entityBase.DateCreated = DateTime.Now;
            }
            insertEntities.Add(entity);
        }

        await DbSet.AddRangeAsync(insertEntities);
        await _zyntraContext.SaveChangesAsync();
        return insertEntities;
    }

    public virtual async Task<TEntity> DeleteAsync(long Id)
    {
        var entity = await _zyntraContext.Set<TEntity>().FindAsync(Id);
        _zyntraContext.Set<TEntity>().Remove(entity);
        await _zyntraContext.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        if (entity is EntityBase entityBase)
        {
            entityBase.DateDeleted = DateTime.Now;
            entityBase.Situation = Situation.Deleted;
        }

        _zyntraContext.Entry(entity).State = EntityState.Modified;
        await _zyntraContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var result = await DbSet.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
        return result;
    }

    public async Task<TEntity?> GetByIdAsync(long Id)
    {
        var result = await _zyntraContext.Set<TEntity>().FindAsync(Id);
        return result;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        if (entity is EntityBase entityBase)
            entityBase.DateModified = DateTime.Now;

        // Detach any tracked entity with the same key to avoid
        // "instance with the key value X is already being tracked"
        if (entity is EntityBase eb && eb.Id > 0)
        {
            var tracked = _zyntraContext.ChangeTracker
                .Entries<TEntity>()
                .FirstOrDefault(e => e.Entity is EntityBase existing && existing.Id == eb.Id && !ReferenceEquals(e.Entity, entity));
            if (tracked != null)
                tracked.State = EntityState.Detached;
        }

        _zyntraContext.Entry(entity).State = EntityState.Modified;
        await _zyntraContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        var updateEntities = new List<TEntity>();
        foreach (var entity in entities)
        {
            if (entity is EntityBase entityBase)
                entityBase.DateModified = DateTime.Now;

            _zyntraContext.Entry(entity).State = EntityState.Modified;
            updateEntities.Add(entity);
        }

        DbSet.UpdateRange(updateEntities);
        await _zyntraContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IList<TEntity> entities)
    {
        _ = entities ?? throw new ArgumentNullException(nameof(entities));

        var deleteEntities = new List<TEntity>();
        foreach (var entity in entities)
        {
            if (entity is EntityBase entityBase)
            {
                entityBase.DateDeleted = DateTime.Now;
                entityBase.Situation = Situation.Deleted;
            }
            _zyntraContext.Entry(entity).State = EntityState.Modified;
            deleteEntities.Add(entity);
        }

        DbSet.UpdateRange(deleteEntities);
        await _zyntraContext.SaveChangesAsync();
    }

}
