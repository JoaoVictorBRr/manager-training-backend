using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.ExerciseDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Instructor;

[ApiController]
[Route("api/exercises")]
[Authorize(Roles = "Instructor,Administrator")]
public class InstructorExerciseController(
    IExerciseService exerciseService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("by-workout/{workoutSheetId}")]
    [SwaggerOperation(Summary = "Listar exercícios de uma ficha de treino")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByWorkout(long workoutSheetId)
    {
        try
        {
            var result = await exerciseService.GetByWorkoutSheetAsync(workoutSheetId);
            return Ok(mapper.Map<IEnumerable<ExerciseResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Adicionar exercício a uma ficha de treino")]
    [ProducesResponseType(typeof(ExerciseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ExerciseCreateDto dto)
    {
        try
        {
            var entity = mapper.Map<Zyntra.Domain.Entities.Exercise>(dto);
            entity.UserIdCreated = currentUserService.GetCurrentUserId();
            var created = await exerciseService.AddAsync(entity);
            return Created($"api/exercises/{created.Id}", mapper.Map<ExerciseResponseDto>(created));
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
    [SwaggerOperation(Summary = "Atualizar exercício")]
    [ProducesResponseType(typeof(ExerciseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] ExerciseUpdateDto dto)
    {
        try
        {
            var entity = await exerciseService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Exercício não encontrado.", StatusCode = 404 });
            mapper.Map(dto, entity);
            entity.UserIdModified = currentUserService.GetCurrentUserId();
            entity.DateModified = DateTime.Now;
            var updated = await exerciseService.UpdateAsync(entity);
            return Ok(mapper.Map<ExerciseResponseDto>(updated));
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

    [HttpDelete("delete/{id}")]
    [SwaggerOperation(Summary = "Remover exercício (soft delete)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var entity = await exerciseService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Exercício não encontrado.", StatusCode = 404 });
            entity.UserIdDeleted = currentUserService.GetCurrentUserId();
            entity.DateDeleted = DateTime.Now;
            await exerciseService.DeleteAsync(entity);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
