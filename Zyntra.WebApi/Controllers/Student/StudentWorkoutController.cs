using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.WorkoutSheetDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/workouts/student")]
[Authorize(Roles = "Student")]
public class StudentWorkoutController(
    IWorkoutSheetService workoutSheetService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("active/{studentId}")]
    [SwaggerOperation(Summary = "Buscar ficha de treino ativa do aluno")]
    [ProducesResponseType(typeof(WorkoutSheetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive(long studentId)
    {
        try
        {
            var sheet = await workoutSheetService.GetActiveByStudentAsync(studentId);
            if (sheet == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Nenhuma ficha de treino ativa encontrada.", StatusCode = 404 });
            return Ok(mapper.Map<WorkoutSheetResponseDto>(sheet));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("get-by-id/{id}")]
    [SwaggerOperation(Summary = "Buscar ficha de treino por ID")]
    [ProducesResponseType(typeof(WorkoutSheetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var entity = await workoutSheetService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Ficha de treino não encontrada.", StatusCode = 404 });
            return Ok(mapper.Map<WorkoutSheetResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
