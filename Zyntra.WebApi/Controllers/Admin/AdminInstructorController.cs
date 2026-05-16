using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Admin;

[ApiController]
[Route("api/instructors")]
[Authorize(Roles = "Administrator")]
public class AdminInstructorController(
    IInstructorService instructorService,
    IMapper mapper,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("get-all")]
    [SwaggerOperation(Summary = "Listar todos os instrutores com filtros de paginação")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] InstructorRequestListDto filter)
    {
        try
        {
            var result = await instructorService.FilterAllInstructors(filter);
            return Ok(mapper.Map<IEnumerable<InstructorResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("get-by-id/{id}")]
    [SwaggerOperation(Summary = "Buscar instrutor por ID")]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var entity = await instructorService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Instrutor não encontrado.", StatusCode = 404 });
            return Ok(mapper.Map<InstructorResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Cadastrar novo instrutor")]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] InstructorCreateDto dto)
    {
        try
        {
            var entity = mapper.Map<Zyntra.Domain.Entities.Instructor>(dto);
            entity.UserIdCreated = currentUserService.GetCurrentUserId();
            var created = await instructorService.AddAsync(entity);
            return Created($"api/instructors/{created.Id}", mapper.Map<InstructorResponseDto>(created));
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
    [SwaggerOperation(Summary = "Atualizar dados do instrutor")]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] InstructorUpdateDto dto)
    {
        try
        {
            var entity = await instructorService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Instrutor não encontrado.", StatusCode = 404 });
            entity.Specialty = dto.Specialty;
            entity.Cref = dto.Cref;
            entity.UserIdModified = currentUserService.GetCurrentUserId();
            entity.DateModified = DateTime.Now;
            var updated = await instructorService.UpdateAsync(entity);
            return Ok(mapper.Map<InstructorResponseDto>(updated));
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
    [SwaggerOperation(Summary = "Remover instrutor (soft delete)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var entity = await instructorService.GetByIdAsync(id);
            if (entity == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Instrutor não encontrado.", StatusCode = 404 });
            entity.UserIdDeleted = currentUserService.GetCurrentUserId();
            entity.DateDeleted = DateTime.Now;
            await instructorService.DeleteAsync(entity);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
