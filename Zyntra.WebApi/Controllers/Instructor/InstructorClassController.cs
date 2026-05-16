using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Instructor;

[ApiController]
[Route("api/classes")]
[Authorize(Roles = "Instructor,Administrator")]
public class InstructorClassController(
    IClassService classService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("get-all")]
    [SwaggerOperation(Summary = "Listar todas as turmas com filtros de paginação")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] ClassRequestListDto filter)
    {
        try
        {
            var result = await classService.FilterAllClasses(filter);
            return Ok(mapper.Map<IEnumerable<ClassResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("available")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Listar turmas disponíveis (com vagas)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailable()
    {
        try
        {
            var result = await classService.GetAvailableClassesAsync();
            return Ok(mapper.Map<IEnumerable<ClassResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("get-by-id/{id}")]
    [SwaggerOperation(Summary = "Buscar turma por ID")]
    [ProducesResponseType(typeof(ClassResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var entity = await classService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Turma não encontrada.", StatusCode = 404 });
            return Ok(mapper.Map<ClassResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Criar nova turma")]
    [ProducesResponseType(typeof(ClassResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ClassCreateDto dto)
    {
        try
        {
            var entity = mapper.Map<Class>(dto);
            entity.AvailableSlots = dto.MaxCapacity;
            entity.UserIdCreated = currentUserService.GetCurrentUserId();
            var created = await classService.AddAsync(entity);
            return Created($"api/classes/{created.Id}", mapper.Map<ClassResponseDto>(created));
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
    [SwaggerOperation(Summary = "Atualizar dados da turma")]
    [ProducesResponseType(typeof(ClassResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] ClassUpdateDto dto)
    {
        try
        {
            var entity = await classService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Turma não encontrada.", StatusCode = 404 });
            mapper.Map(dto, entity);
            entity.UserIdModified = currentUserService.GetCurrentUserId();
            entity.DateModified = DateTime.Now;
            var updated = await classService.UpdateAsync(entity);
            return Ok(mapper.Map<ClassResponseDto>(updated));
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
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Remover turma (soft delete)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var entity = await classService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Turma não encontrada.", StatusCode = 404 });
            entity.UserIdDeleted = currentUserService.GetCurrentUserId();
            entity.DateDeleted = DateTime.Now;
            await classService.DeleteAsync(entity);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
