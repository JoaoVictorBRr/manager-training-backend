using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using Zyntra.Domain.Dtos.HydrationDto;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/hydration")]
[Authorize]
public class StudentHydrationController(
    IHydrationService hydrationService,
    IStudentService studentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("today")]
    [SwaggerOperation(Summary = "Obter resumo de hidratação de hoje")]
    [ProducesResponseType(typeof(HydrationSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetToday()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            // Goal from onboarding (WaterIntake in liters → convert to ml)
            decimal goalMl = 2000;
            if (!string.IsNullOrEmpty(student.OnboardingDataJson))
            {
                var data = JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (data?.WaterIntake.HasValue == true)
                    goalMl = data.WaterIntake.Value * 1000;
                else if (data?.Weight.HasValue == true)
                    goalMl = data.Weight.Value * 35;
            }

            var summary = await hydrationService.GetTodaySummaryAsync(student.Id, goalMl);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("log")]
    [SwaggerOperation(Summary = "Registrar consumo de água")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Log([FromBody] AddHydrationDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            await hydrationService.AddLogAsync(student.Id, dto.AmountMl);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
