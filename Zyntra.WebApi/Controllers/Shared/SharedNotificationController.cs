using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.NotificationDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Shared;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class SharedNotificationController(
    INotificationService notificationService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("get-by-user/{userId}")]
    [SwaggerOperation(Summary = "Listar notificações do usuário")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByUser(long userId)
    {
        try
        {
            var result = await notificationService.GetByUserAsync(userId);
            return Ok(mapper.Map<IEnumerable<NotificationResponseDto>>(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("mark-all-as-read/{userId}")]
    [SwaggerOperation(Summary = "Marcar todas as notificações do usuário como lidas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAllAsRead(long userId)
    {
        try
        {
            var notifications = await notificationService.GetAllAsync(n => n.UserId == userId && !n.IsRead);
            foreach (var n in notifications)
            {
                n.IsRead = true;
                await notificationService.UpdateAsync(n);
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("mark-as-read/{id}")]
    [SwaggerOperation(Summary = "Marcar notificação como lida")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        try
        {
            await notificationService.MarkAsReadAsync(id);
            return NoContent();
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
}
