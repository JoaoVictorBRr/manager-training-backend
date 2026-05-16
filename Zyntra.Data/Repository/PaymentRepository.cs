using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.PaymentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class PaymentRepository(ZyntraContext zyntraContext) : BaseRepository<Payment>(zyntraContext), IPaymentRepository
{
    public async Task<PagedListDto<Payment>> FilterAllPayments(PaymentRequestListDto filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(p => p.Student).ThenInclude(s => s.User)
            .Where(p => filter.Situation == Situation.ForFilterActiveAndInactive || p.Situation == filter.Situation);

        if (filter.StudentId.HasValue)
            query = query.Where(p => p.StudentId == filter.StudentId.Value);

        if (filter.PaymentStatus.HasValue)
            query = query.Where(p => p.PaymentStatus == filter.PaymentStatus.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.DueDate).Skip(filter.Start).Take(filter.Take).ToListAsync();
        return new PagedListDto<Payment>(items, filter.Start, filter.Take, total);
    }

    public async Task<IEnumerable<Payment>> GetOverdueByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.StudentId == studentId && p.PaymentStatus == PaymentStatus.Overdue)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }
}
