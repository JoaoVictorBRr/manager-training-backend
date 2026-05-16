using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.PaymentDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Student")]
public class StudentPaymentController(
    IPaymentService paymentService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("status/{studentId}")]
    [SwaggerOperation(Summary = "Verificar status de pagamento do aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatus(long studentId)
    {
        try
        {
            var status = await paymentService.GetStatusByStudentAsync(studentId);
            return Ok(new { status });
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

    [HttpGet("history/{studentId}")]
    [SwaggerOperation(Summary = "Listar histórico de pagamentos do aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistory(long studentId)
    {
        try
        {
            var result = await paymentService.GetHistoryByStudentAsync(studentId);
            return Ok(mapper.Map<IEnumerable<PaymentResponseDto>>(result));
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
