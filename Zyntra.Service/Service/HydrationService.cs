using System.Linq.Expressions;
using FluentValidation.Results;
using Zyntra.Domain.Dtos.HydrationDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class HydrationService(IHydrationLogRepository repo) : IHydrationService
{
    public async Task<HydrationSummaryDto> GetTodaySummaryAsync(long studentId, decimal goalMl)
    {
        var logs = (await repo.GetTodayLogsAsync(studentId)).ToList();
        var totalMl = logs.Sum(l => l.AmountMl);
        var percentage = goalMl > 0 ? Math.Min(100, (totalMl / goalMl) * 100) : 0;

        return new HydrationSummaryDto
        {
            TotalMl = totalMl,
            GoalMl = goalMl,
            PercentageAchieved = Math.Round(percentage, 1),
            Logs = logs.Select(l => new HydrationLogDto
            {
                Id = l.Id,
                LogDate = l.LogDate,
                AmountMl = l.AmountMl,
            }).ToList(),
        };
    }

    public async Task AddLogAsync(long studentId, decimal amountMl)
    {
        var log = new HydrationLog
        {
            StudentId = studentId,
            LogDate = DateTime.Today,
            AmountMl = amountMl,
        };
        await repo.AddAsync(log);
    }

    // IServiceBase boilerplate
    public Task<HydrationLog> AddAsync(HydrationLog entity) => repo.AddAsync(entity);
    public Task AddRangeAsync(IList<HydrationLog> entity) => repo.AddRangeAsync(entity);
    public Task<IList<HydrationLog>> AddRangeListAsync(IList<HydrationLog> entities) => repo.AddRangeListAsync(entities);
    public Task<HydrationLog> UpdateAsync(HydrationLog entity) => repo.UpdateAsync(entity);
    public Task UpdateRangeAsync(IEnumerable<HydrationLog> entity) => repo.UpdateRangeAsync(entity);
    public Task<HydrationLog> DeleteAsync(HydrationLog entity) => repo.DeleteAsync(entity);
    public Task<HydrationLog> DeleteAsync(long Id) => repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<HydrationLog> entities) => repo.DeleteRangeAsync(entities);
    public Task<HydrationLog> GetByIdAsync(long Id) => repo.GetByIdAsync(Id);
    public Task<IEnumerable<HydrationLog>> GetAllAsync(Expression<Func<HydrationLog, bool>> predicate) => repo.GetAllAsync(predicate);
    public Task<HydrationLog> GetAsync(Expression<Func<HydrationLog, bool>> predicate) => repo.GetAsync(predicate);
    public Task<List<ValidationFailure>> Validate(HydrationLog entity) => Task.FromResult(new List<ValidationFailure>());
}
