using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class PaymentService(IPaymentRepository repo, IValidator<Payment> validator) : IPaymentService
{
    private readonly IPaymentRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Payment> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Payment> AddAsync(Payment entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Payment> UpdateAsync(Payment entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Payment> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Payment>> AddRangeListAsync(IList<Payment> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Payment> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Payment> DeleteAsync(Payment entity) => _repo.DeleteAsync(entity);
    public Task<Payment> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Payment> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Payment> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Payment> GetAsync(Expression<Func<Payment, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<PaymentStatus> GetStatusByStudentAsync(long studentId)
    {
        var overdue = await _repo.GetOverdueByStudentAsync(studentId);
        if (overdue.Any())
            return PaymentStatus.Overdue;

        var payments = await _repo.GetByStudentAsync(studentId);
        var latest = payments.OrderByDescending(p => p.DueDate).FirstOrDefault();
        return latest?.PaymentStatus ?? PaymentStatus.Pending;
    }

    public Task<IEnumerable<Payment>> GetHistoryByStudentAsync(long studentId)
        => _repo.GetByStudentAsync(studentId);

    public async Task<bool> IsStudentOverdueAsync(long studentId)
    {
        var overdue = await _repo.GetOverdueByStudentAsync(studentId);
        return overdue.Any();
    }

    public async Task<List<ValidationFailure>> Validate(Payment entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
