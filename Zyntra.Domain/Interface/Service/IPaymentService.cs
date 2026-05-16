using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IPaymentService : IServiceBase<Payment>
{
    Task<PaymentStatus> GetStatusByStudentAsync(long studentId);
    Task<IEnumerable<Payment>> GetHistoryByStudentAsync(long studentId);
    Task<bool> IsStudentOverdueAsync(long studentId);
}
