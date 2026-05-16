using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.ChatDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Shared;

[ApiController]
[Route("api/chat")]
[Authorize]
public class SharedChatController(
    IChatService chatService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("conversation/{studentId}/{instructorId}")]
    [SwaggerOperation(Summary = "Listar mensagens de uma conversa entre aluno e instrutor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConversation(long studentId, long instructorId)
    {
        try
        {
            var result = await chatService.GetConversationAsync(studentId, instructorId);
            return Ok(mapper.Map<IEnumerable<ChatMessageResponseDto>>(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Type = "NotFound", Message = ex.Message, StatusCode = 404 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("send")]
    [SwaggerOperation(Summary = "Enviar mensagem no chat")]
    [ProducesResponseType(typeof(ChatMessageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Send([FromBody] ChatMessageSendDto dto)
    {
        try
        {
            var message = await chatService.SendMessageAsync(dto);
            return Created(string.Empty, mapper.Map<ChatMessageResponseDto>(message));
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
