using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class CheckInService(
    ICheckInRepository repo,
    IValidator<CheckIn> validator,
    IStudentRepository studentRepository,
    IPaymentRepository paymentRepository,
    IPartnerIntegrationRepository partnerIntegrationRepository) : ICheckInService
{
    private readonly ICheckInRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<CheckIn> _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IStudentRepository _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
    private readonly IPaymentRepository _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    private readonly IPartnerIntegrationRepository _partnerIntegrationRepository = partnerIntegrationRepository ?? throw new ArgumentNullException(nameof(partnerIntegrationRepository));

    public async Task<CheckIn> AddAsync(CheckIn entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<CheckIn> UpdateAsync(CheckIn entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<CheckIn> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<CheckIn>> AddRangeListAsync(IList<CheckIn> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<CheckIn> entity) => _repo.UpdateRangeAsync(entity);
    public Task<CheckIn> DeleteAsync(CheckIn entity) => _repo.DeleteAsync(entity);
    public Task<CheckIn> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<CheckIn> entities) => _repo.DeleteRangeAsync(entities);
    public Task<CheckIn> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<CheckIn>> GetAllAsync(Expression<Func<CheckIn, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<CheckIn> GetAsync(Expression<Func<CheckIn, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<CheckIn> CheckInAsync(CheckInRequestDto request)
    {
        if (request.AccessType != CheckInType.App)
        {
            var overduePayments = await _paymentRepository.GetOverdueByStudentAsync(request.StudentId);
            if (overduePayments.Any())
                throw new InvalidOperationException("Check-in bloqueado: aluno com pagamentos em atraso.");
        }

        var checkIn = new CheckIn
        {
            StudentId = request.StudentId,
            DateTime = DateTime.Now,
            Unit = "Principal",
            AccessType = request.AccessType,
            ValidationStatus = CheckInStatus.Approved,
            WorkoutDayPerformed = request.WorkoutDayPerformed
        };

        return await this.AddAsync(checkIn);
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

        return await this.AddAsync(checkIn);
    }

    public async Task<List<ValidationFailure>> Validate(CheckIn entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
