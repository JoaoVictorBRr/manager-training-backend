using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.PaymentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IPaymentRepository : IRepositoryBase<Payment>
{
    Task<PagedListDto<Payment>> FilterAllPayments(PaymentRequestListDto filter);
    Task<IEnumerable<Payment>> GetOverdueByStudentAsync(long studentId);
    Task<IEnumerable<Payment>> GetByStudentAsync(long studentId);
}
