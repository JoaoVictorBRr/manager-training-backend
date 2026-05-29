using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.WorkoutSessionDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/workout-session")]
[Authorize]
public class StudentWorkoutSessionController(
    IWorkoutSessionService workoutSessionService,
    IStudentService studentService,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    [HttpPost("start")]
    [SwaggerOperation(Summary = "Iniciar sessão de treino")]
    [ProducesResponseType(typeof(WorkoutSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartSession([FromBody] StartWorkoutSessionDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var session = await workoutSessionService.StartSessionAsync(student.Id, dto.WorkoutDay);
            return Ok(mapper.Map<WorkoutSessionResponseDto>(session));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("{id}/exercise")]
    [SwaggerOperation(Summary = "Registrar exercício concluído na sessão")]
    [ProducesResponseType(typeof(ExerciseLogResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LogExercise(long id, [FromBody] LogExerciseDto dto)
    {
        try
        {
            var log = await workoutSessionService.LogExerciseAsync(id, dto);
            return Ok(mapper.Map<ExerciseLogResponseDto>(log));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Type = "NotFound", Message = ex.Message, StatusCode = 404 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("{id}/finish")]
    [SwaggerOperation(Summary = "Finalizar sessão de treino")]
    [ProducesResponseType(typeof(WorkoutSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinishSession(long id, [FromQuery] long? checkInId)
    {
        try
        {
            var session = await workoutSessionService.FinishSessionAsync(id, checkInId);
            return Ok(mapper.Map<WorkoutSessionResponseDto>(session));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Type = "NotFound", Message = ex.Message, StatusCode = 404 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("active")]
    [SwaggerOperation(Summary = "Retorna sessão de treino ativa do aluno")]
    [ProducesResponseType(typeof(WorkoutSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var session = await workoutSessionService.GetActiveSessionAsync(student.Id);
            if (session == null) return NoContent();
            return Ok(mapper.Map<WorkoutSessionResponseDto>(session));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("exercise-last-log")]
    [SwaggerOperation(Summary = "Último registro de um exercício pelo nome")]
    [ProducesResponseType(typeof(ExerciseLogResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLastExerciseLog([FromQuery] string exerciseName)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var log = await workoutSessionService.GetLastExerciseLogAsync(student.Id, exerciseName);
            if (log == null) return NoContent();
            return Ok(mapper.Map<ExerciseLogResponseDto>(log));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("history")]
    [SwaggerOperation(Summary = "Histórico de sessões de treino concluídas")]
    [ProducesResponseType(typeof(IEnumerable<WorkoutSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistory()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var sessions = await workoutSessionService.GetCompletedSessionsAsync(student.Id);
            return Ok(mapper.Map<IEnumerable<WorkoutSessionResponseDto>>(sessions));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
