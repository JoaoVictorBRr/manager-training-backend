using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.PaymentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Student")]
public class StudentPaymentController(
    IPaymentService paymentService,
    IStudentService studentService,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    [HttpPost("subscribe")]
    [SwaggerOperation(Summary = "Assinar plano de acesso")]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribePlanDto dto)
    {
        try
        {
            if (dto.PlanId < 1 || dto.PlanId > 3)
                return BadRequest(new ErrorResponse { Type = "ValidationError", Message = "Plano inválido.", StatusCode = 400 });

            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            decimal amount = dto.PlanId switch { 1 => 49.90m, 2 => 79.90m, _ => 119.90m };

            var payment = await paymentService.AddAsync(new Payment
            {
                StudentId = student.Id,
                Amount = amount,
                DueDate = DateTime.UtcNow.AddMonths(1),
                PaymentDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = $"Cartão de crédito ****{dto.LastFourDigits}",
            });

            student.SubscriptionPlan = dto.PlanId;
            await studentService.UpdateAsync(student);

            return Created("/api/payments/history", mapper.Map<PaymentResponseDto>(payment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

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
