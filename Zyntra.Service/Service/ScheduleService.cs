using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class ScheduleService(
    IScheduleRepository scheduleRepository,
    IClassRepository classRepository,
    IWaitListRepository waitListRepository,
    IPaymentRepository paymentRepository) : BaseService<Schedule>(scheduleRepository), IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository = scheduleRepository;
    private readonly IClassRepository _classRepository = classRepository;
    private readonly IWaitListRepository _waitListRepository = waitListRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public Task<PagedListDto<Schedule>> FilterAllSchedules(ScheduleRequestListDto filter)
        => _scheduleRepository.FilterAllSchedules(filter);

    public async Task<Schedule> ReserveAsync(long studentId, long classId)
    {
        var overduePayments = await _paymentRepository.GetOverdueByStudentAsync(studentId);
        if (overduePayments.Any())
            throw new InvalidOperationException("Aluno com pagamentos em atraso não pode reservar aulas.");

        var existing = await _scheduleRepository.GetByStudentAndClassAsync(studentId, classId);
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

        return await _scheduleRepository.AddAsync(schedule);
    }

    public async Task<Schedule> CancelAsync(long scheduleId)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null)
            throw new InvalidOperationException("Reserva não encontrada.");

        schedule.Status = ScheduleStatus.Cancelled;
        await _scheduleRepository.UpdateAsync(schedule);

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
                await _scheduleRepository.AddAsync(newSchedule);
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
}
