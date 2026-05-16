using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class CheckInService(
    ICheckInRepository checkInRepository,
    IStudentRepository studentRepository,
    IPaymentRepository paymentRepository,
    IPartnerIntegrationRepository partnerIntegrationRepository) : BaseService<CheckIn>(checkInRepository), ICheckInService
{
    private readonly ICheckInRepository _checkInRepository = checkInRepository;
    private readonly IStudentRepository _studentRepository = studentRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPartnerIntegrationRepository _partnerIntegrationRepository = partnerIntegrationRepository;

    public async Task<CheckIn> CheckInAsync(CheckInRequestDto request)
    {
        var overduePayments = await _paymentRepository.GetOverdueByStudentAsync(request.StudentId);
        if (overduePayments.Any())
            throw new InvalidOperationException("Check-in bloqueado: aluno com pagamentos em atraso.");

        var checkIn = new CheckIn
        {
            StudentId = request.StudentId,
            DateTime = DateTime.Now,
            Unit = "Principal",
            AccessType = request.AccessType,
            ValidationStatus = CheckInStatus.Approved
        };

        return await _checkInRepository.AddAsync(checkIn);
    }

    public async Task<CheckIn> CheckInPartnerAsync(CheckInPartnerRequestDto request)
    {
        var partnerType = request.PartnerType == CheckInType.Gympass ? PartnerType.Gympass : PartnerType.TotalPass;
        var partner = await _partnerIntegrationRepository.GetByTokenAsync(request.PartnerToken, partnerType);

        if (partner == null || partner.ValidationStatus != CheckInStatus.Approved)
            throw new InvalidOperationException("Token de parceiro inválido ou não autorizado.");

        var checkIn = new CheckIn
        {
            StudentId = 0,
            DateTime = DateTime.Now,
            Unit = "Principal",
            AccessType = request.PartnerType,
            ValidationStatus = CheckInStatus.Approved
        };

        return await _checkInRepository.AddAsync(checkIn);
    }
}
