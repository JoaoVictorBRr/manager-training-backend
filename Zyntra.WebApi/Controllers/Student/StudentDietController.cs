using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.DietDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/diet")]
[Authorize]
public class StudentDietController(
    IStudentDietService dietService,
    IStudentService studentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Obter plano alimentar ativo do aluno")]
    [ProducesResponseType(typeof(StudentDietResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var diet = await dietService.GetActiveDietAsync(student.Id);
            if (diet == null) return NoContent();
            return Ok(diet);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Criar plano alimentar do aluno")]
    [ProducesResponseType(typeof(StudentDietResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateDietDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var diet = await dietService.CreateDietAsync(student.Id, dto);
            return Created($"/api/diet/active", diet);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("meal/{mealId}/complete")]
    [SwaggerOperation(Summary = "Marcar refeição como concluída")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteMeal(long mealId)
    {
        try
        {
            await dietService.CompleteMealAsync(mealId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
