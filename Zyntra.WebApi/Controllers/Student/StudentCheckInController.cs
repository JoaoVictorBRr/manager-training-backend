using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/checkin")]
[Authorize]
public class StudentCheckInController(
    ICheckInService checkInService,
    IStudentService studentService,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("my-history")]
    [SwaggerOperation(Summary = "Listar meu histórico de check-ins")]
    [ProducesResponseType(typeof(IEnumerable<CheckInResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyHistory()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var result = await checkInService.GetAllAsync(c => c.StudentId == student.Id);
            return Ok(mapper.Map<IEnumerable<CheckInResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("history/{studentId}")]
    [SwaggerOperation(Summary = "Listar histórico de check-ins do aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistory(long studentId)
    {
        try
        {
            var result = await checkInService.GetAllAsync(c => c.StudentId == studentId);
            return Ok(mapper.Map<IEnumerable<CheckInResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("check-in")]
    [SwaggerOperation(Summary = "Realizar check-in na academia")]
    [ProducesResponseType(typeof(CheckInResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            dto.StudentId = student.Id;
            var result = await checkInService.CheckInAsync(dto);
            return Ok(mapper.Map<CheckInResponseDto>(result));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Type = "ValidationError",
                Message = "Dados inválidos.",
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList(),
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new ErrorResponse { Type = "UnprocessableEntity", Message = ex.Message, StatusCode = 422 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("partner")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Realizar check-in via parceiro (Gympass, TotalPass)")]
    [ProducesResponseType(typeof(CheckInResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckInPartner([FromBody] CheckInPartnerRequestDto dto)
    {
        try
        {
            var result = await checkInService.CheckInPartnerAsync(dto);
            return Ok(mapper.Map<CheckInResponseDto>(result));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Type = "ValidationError",
                Message = "Dados inválidos.",
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList(),
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new ErrorResponse { Type = "UnprocessableEntity", Message = ex.Message, StatusCode = 422 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
