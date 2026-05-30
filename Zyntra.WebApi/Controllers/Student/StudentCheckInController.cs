using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/checkin")]
[Authorize(Roles = "Student")]
public class StudentCheckInController(
    ICheckInService checkInService,
    ICurrentUserService currentUserService,
    IStudentService studentService) : ControllerBase
{
    [HttpPost("check-in")]
    [SwaggerOperation(Summary = "Registrar check-in ao finalizar treino")]
    [ProducesResponseType(typeof(CheckInHistoryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException("Aluno não encontrado.");

            var result = await checkInService.CheckInAsync(student.Id, dto.WorkoutDay);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse(ex.Message));
        }
    }

    [HttpGet("my-history")]
    [SwaggerOperation(Summary = "Histórico de check-ins do aluno")]
    [ProducesResponseType(typeof(IEnumerable<CheckInHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyHistory()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId)
                ?? throw new InvalidOperationException("Aluno não encontrado.");

            var result = await checkInService.GetHistoryAsync(student.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse(ex.Message));
        }
    }
}
