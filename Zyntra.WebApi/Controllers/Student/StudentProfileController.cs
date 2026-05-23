using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentProfileController(
    IStudentService studentService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Obter perfil do aluno autenticado")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Perfil de aluno não encontrado.", StatusCode = 404 });
            return Ok(mapper.Map<StudentResponseDto>(student));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("me/onboarding")]
    [SwaggerOperation(Summary = "Salvar dados do onboarding do aluno autenticado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveOnboarding([FromBody] SaveOnboardingDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            await studentService.SaveOnboardingAsync(userId, dto);
            return NoContent();
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
}
