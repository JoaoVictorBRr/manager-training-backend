using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class PaymentService(IPaymentRepository paymentRepository) : BaseService<Payment>(paymentRepository), IPaymentService
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<PaymentStatus> GetStatusByStudentAsync(long studentId)
    {
        var overdue = await _paymentRepository.GetOverdueByStudentAsync(studentId);
        if (overdue.Any())
            return PaymentStatus.Overdue;

        var payments = await _paymentRepository.GetByStudentAsync(studentId);
        var latest = payments.OrderByDescending(p => p.DueDate).FirstOrDefault();
        return latest?.PaymentStatus ?? PaymentStatus.Pending;
    }

    public Task<IEnumerable<Payment>> GetHistoryByStudentAsync(long studentId)
        => _paymentRepository.GetByStudentAsync(studentId);

    public async Task<bool> IsStudentOverdueAsync(long studentId)
    {
        var overdue = await _paymentRepository.GetOverdueByStudentAsync(studentId);
        return overdue.Any();
    }
}
