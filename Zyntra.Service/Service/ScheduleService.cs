using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class ScheduleService(
    IScheduleRepository repo,
    IValidator<Schedule> validator,
    IClassRepository classRepository,
    IWaitListRepository waitListRepository,
    IPaymentRepository paymentRepository) : IScheduleService
{
    private readonly IScheduleRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Schedule> _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IClassRepository _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
    private readonly IWaitListRepository _waitListRepository = waitListRepository ?? throw new ArgumentNullException(nameof(waitListRepository));
    private readonly IPaymentRepository _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));

    public async Task<Schedule> AddAsync(Schedule entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Schedule> UpdateAsync(Schedule entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Schedule> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Schedule>> AddRangeListAsync(IList<Schedule> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Schedule> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Schedule> DeleteAsync(Schedule entity) => _repo.DeleteAsync(entity);
    public Task<Schedule> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Schedule> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Schedule> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Schedule>> GetAllAsync(Expression<Func<Schedule, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Schedule> GetAsync(Expression<Func<Schedule, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<Schedule>> FilterAllSchedules(ScheduleRequestListDto filter)
        => _repo.FilterAllSchedules(filter);

    public async Task<Schedule> ReserveAsync(long studentId, long classId)
    {
        var overduePayments = await _paymentRepository.GetOverdueByStudentAsync(studentId);
        if (overduePayments.Any())
            throw new InvalidOperationException("Aluno com pagamentos em atraso não pode reservar aulas.");

        var existing = await _repo.GetByStudentAndClassAsync(studentId, classId);
        if (existing != null)
            throw new InvalidOperationException("Aluno já possui reserva nesta aula.");

        var gymClass = await _classRepository.GetByIdAsync(classId);
        if (gymClass == null)
            throw new InvalidOperationException("Aula não encontrada.");

        if (gymClass.AvailableSlots <= 0)
            throw new InvalidOperationException("Não há vagas disponíveis. Considere entrar na lista de espera.");

        gymClass.AvailableSlots--;
        await _classRepository.UpdateAsync(gymClass);

        var schedule = new Schedule
        {
            StudentId = studentId,
            ClassId = classId,
            Status = ScheduleStatus.Confirmed,
            ReservationDate = DateTime.Now
        };

        return await _repo.AddAsync(schedule);
    }

    public async Task<Schedule> CancelAsync(long scheduleId)
    {
        var schedule = await _repo.GetByIdAsync(scheduleId);
        if (schedule == null)
            throw new InvalidOperationException("Reserva não encontrada.");

        schedule.Status = ScheduleStatus.Cancelled;
        await _repo.UpdateAsync(schedule);

        var gymClass = await _classRepository.GetByIdAsync(schedule.ClassId);
        if (gymClass != null)
        {
            gymClass.AvailableSlots++;
            await _classRepository.UpdateAsync(gymClass);

            var nextInLine = await _waitListRepository.GetFirstInLineAsync(schedule.ClassId);
            if (nextInLine != null)
            {
                var newSchedule = new Schedule
                {
                    StudentId = nextInLine.StudentId,
                    ClassId = schedule.ClassId,
                    Status = ScheduleStatus.Confirmed,
                    ReservationDate = DateTime.Now
                };
                await _repo.AddAsync(newSchedule);
                await _waitListRepository.DeleteAsync(nextInLine);
                gymClass.AvailableSlots--;
                await _classRepository.UpdateAsync(gymClass);
            }
        }

        return schedule;
    }

    public async Task<WaitList> JoinWaitListAsync(long studentId, long classId)
    {
        var existing = await _waitListRepository.GetByStudentAndClassAsync(studentId, classId);
        if (existing != null)
            throw new InvalidOperationException("Aluno já está na lista de espera desta aula.");

        var lastPosition = (await _waitListRepository.GetAllAsync(w => w.ClassId == classId))
            .Select(w => w.Position)
            .DefaultIfEmpty(0)
            .Max();

        var waitList = new WaitList
        {
            StudentId = studentId,
            ClassId = classId,
            Position = lastPosition + 1,
            InclusionDateTime = DateTime.Now
        };

        return await _waitListRepository.AddAsync(waitList);
    }

    public async Task<List<ValidationFailure>> Validate(Schedule entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
