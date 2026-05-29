using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zyntra.Domain.Dtos.EvolutionPhotoDto;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/upload")]
[Authorize]
public class StudentUploadController(
    IEvolutionPhotoService evolutionPhotoService,
    IStudentDietService dietService,
    IStudentService studentService,
    ICurrentUserService currentUserService,
    IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("evolution-photo")]
    [SwaggerOperation(Summary = "Upload de foto de evolução")]
    [ProducesResponseType(typeof(EvolutionPhotoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadEvolutionPhoto(IFormFile file, [FromForm] string? notes = null)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ErrorResponse { Type = "BadRequest", Message = "Arquivo não enviado.", StatusCode = 400 });

            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var folder = Path.Combine(env.WebRootPath, "images", "evolutionPhotos", student.Id.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = $"/images/evolutionPhotos/{student.Id}/{fileName}";
            var result = await evolutionPhotoService.AddPhotoAsync(student.Id, relativePath, notes);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("evolution-photos")]
    [SwaggerOperation(Summary = "Listar fotos de evolução do aluno")]
    [ProducesResponseType(typeof(IEnumerable<EvolutionPhotoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEvolutionPhotos()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var photos = await evolutionPhotoService.GetByStudentAsync(student.Id);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("avatar")]
    [SwaggerOperation(Summary = "Upload de avatar do aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ErrorResponse { Type = "BadRequest", Message = "Arquivo não enviado.", StatusCode = 400 });

            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var folder = Path.Combine(env.WebRootPath, "images", "avatars", student.Id.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = $"/images/avatars/{student.Id}/{fileName}";
            student.AvatarUrl = relativePath;
            await studentService.UpdateAsync(student);

            return Ok(new { avatarUrl = relativePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("meal-photo/{optionId}")]
    [SwaggerOperation(Summary = "Upload de foto de refeição")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadMealPhoto(long optionId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ErrorResponse { Type = "BadRequest", Message = "Arquivo não enviado.", StatusCode = 400 });

            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var folder = Path.Combine(env.WebRootPath, "images", "foodStudents", student.Id.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = $"/images/foodStudents/{student.Id}/{fileName}";
            await dietService.AddMealPhotoAsync(optionId, relativePath);

            return Ok(new { imageUrl = relativePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
