using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.WorkoutSheetDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Instructor;

[ApiController]
[Route("api/workouts")]
[Authorize(Roles = "Instructor,Administrator")]
public class InstructorWorkoutController(
    IWorkoutSheetService workoutSheetService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("create")]
    [SwaggerOperation(Summary = "Criar nova ficha de treino para um aluno")]
    [ProducesResponseType(typeof(WorkoutSheetResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] WorkoutSheetCreateDto dto)
    {
        try
        {
            var entity = new WorkoutSheet
            {
                StudentId = dto.StudentId,
                InstructorId = dto.InstructorId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                Notes = dto.Notes,
                UserIdCreated = currentUserService.GetCurrentUserId()
            };
            var created = await workoutSheetService.AddAsync(entity);
            return Created($"api/workouts/{created.Id}", mapper.Map<WorkoutSheetResponseDto>(created));
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

    [HttpPut("update/{id}")]
    [SwaggerOperation(Summary = "Atualizar ficha de treino")]
    [ProducesResponseType(typeof(WorkoutSheetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] WorkoutSheetUpdateDto dto)
    {
        try
        {
            var entity = await workoutSheetService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Ficha de treino não encontrada.", StatusCode = 404 });
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;
            entity.Notes = dto.Notes;
            entity.UserIdModified = currentUserService.GetCurrentUserId();
            entity.DateModified = DateTime.Now;
            var updated = await workoutSheetService.UpdateAsync(entity);
            return Ok(mapper.Map<WorkoutSheetResponseDto>(updated));
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
