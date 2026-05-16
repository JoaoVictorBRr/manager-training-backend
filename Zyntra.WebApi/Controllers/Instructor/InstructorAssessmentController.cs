using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.PhysicalAssessmentDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Instructor;

[ApiController]
[Route("api/assessments")]
[Authorize(Roles = "Instructor,Administrator")]
public class InstructorAssessmentController(
    IPhysicalAssessmentService assessmentService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("history/{studentId}")]
    [SwaggerOperation(Summary = "Listar histórico de avaliações físicas de um aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistory(long studentId)
    {
        try
        {
            var result = await assessmentService.GetHistoryByStudentAsync(studentId);
            return Ok(mapper.Map<IEnumerable<PhysicalAssessmentResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Registrar nova avaliação física para um aluno")]
    [ProducesResponseType(typeof(PhysicalAssessmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] PhysicalAssessmentCreateDto dto)
    {
        try
        {
            var entity = mapper.Map<Zyntra.Domain.Entities.PhysicalAssessment>(dto);
            entity.UserIdCreated = currentUserService.GetCurrentUserId();
            var created = await assessmentService.AddAsync(entity);
            return Created($"api/assessments/{created.Id}", mapper.Map<PhysicalAssessmentResponseDto>(created));
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
