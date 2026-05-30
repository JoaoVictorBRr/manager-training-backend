using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using Zyntra.Domain.Dtos.PhysicalAssessmentDto;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Dtos.WorkoutSheetDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Helpers;
using Zyntra.Shared.Models;

namespace Zyntra.WebApi.Controllers.Student;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentProfileController(
    IStudentService studentService,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IPhysicalAssessmentRepository assessmentRepository,
    IWorkoutTemplateService workoutTemplateService,
    IStudentDietService dietService) : ControllerBase
{
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Obter perfil do aluno autenticado")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Perfil de aluno não encontrado.", StatusCode = 404 });

            var dto = mapper.Map<StudentResponseDto>(student);
            dto.MembershipStatus = student.Situation == Situation.Active ? "active" : "inactive";
            dto.OnboardingCompleted = student.OnboardingCompleted;

            if (!string.IsNullOrEmpty(student.OnboardingDataJson))
                dto.OnboardingData = JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("me/stats")]
    [SwaggerOperation(Summary = "Obter estatísticas do aluno autenticado")]
    [ProducesResponseType(typeof(StudentStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Perfil de aluno não encontrado.", StatusCode = 404 });

            var stats = await studentService.GetStatsAsync(student);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("me/achievements")]
    [SwaggerOperation(Summary = "Obter conquistas computadas do aluno autenticado")]
    [ProducesResponseType(typeof(IEnumerable<AchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAchievements()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Perfil de aluno não encontrado.", StatusCode = 404 });

            var achievements = await studentService.GetAchievementsAsync(student.Id);
            return Ok(achievements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("me/checkin-weekly-history")]
    [SwaggerOperation(Summary = "Obter histórico semanal de check-ins para o gráfico de performance")]
    [ProducesResponseType(typeof(IEnumerable<WeeklyCheckInDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCheckInWeeklyHistory()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Perfil de aluno não encontrado.", StatusCode = 404 });

            var history = await studentService.GetWeeklyCheckInHistoryAsync(student.Id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("me/onboarding")]
    [SwaggerOperation(Summary = "Salvar dados do onboarding do aluno autenticado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveOnboarding([FromBody] SaveOnboardingDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            await studentService.SaveOnboardingAsync(userId, dto);

            var student = await studentService.GetByUserIdAsync(userId);
            if (student != null)
            {
                var days = dto.TrainingDays ?? 4;
                try { await workoutTemplateService.GenerateWorkoutAsync(student.Id, days); } catch { }
                try { await dietService.GenerateDefaultDietAsync(student.Id, dto.Objective, dto.DietRestrictions); } catch { }
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Type = "NotFound", Message = ex.Message, StatusCode = 404 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("me/profile")]
    [SwaggerOperation(Summary = "Atualizar dados pessoais do aluno autenticado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var user = await userRepository.GetByIdAsync(student.UserId);
            if (user == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Usuário não encontrado.", StatusCode = 404 });

            user.Name = dto.Name;
            user.CellphoneNumber = dto.CellphoneNumber;
            student.BirthDate = dto.BirthDate;

            await userRepository.UpdateAsync(user);
            await studentService.UpdateAsync(student);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPut("me/password")]
    [SwaggerOperation(Summary = "Alterar senha do aluno autenticado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var user = await userRepository.GetByIdAsync(student.UserId);
            if (user == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Usuário não encontrado.", StatusCode = 404 });

            if (!PasswordHasher.VerifyPassword(dto.CurrentPassword, user.Password, user.Salt))
                return BadRequest(new ErrorResponse { Type = "ValidationError", Message = "Senha atual incorreta.", StatusCode = 400 });

            var (hash, salt, _) = PasswordHasher.HashPassword(dto.NewPassword);
            user.Password = hash;
            user.Salt = salt;
            await userRepository.UpdateAsync(user);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("me/body-measurement")]
    [SwaggerOperation(Summary = "Adicionar nova medição corporal do aluno autenticado")]
    [ProducesResponseType(typeof(LatestAssessmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBodyMeasurement([FromBody] AddBodyMeasurementDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var heightM = dto.Height > 10 ? dto.Height / 100m : dto.Height;
            var bmi = Math.Round(dto.Weight / (heightM * heightM), 2);

            var assessment = await assessmentRepository.AddAsync(new PhysicalAssessment
            {
                StudentId = student.Id,
                AssessmentDate = DateTime.UtcNow,
                Weight = dto.Weight,
                Height = heightM,
                Bmi = bmi,
                Notes = dto.Notes ?? string.Empty,
                Measurements = dto.Measurements ?? string.Empty,
            });

            // Update onboarding data with new weight/height
            if (!string.IsNullOrEmpty(student.OnboardingDataJson))
            {
                try
                {
                    var onboarding = JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SaveOnboardingDto();
                    onboarding.Weight = dto.Weight;
                    onboarding.Height = dto.Height;
                    student.OnboardingDataJson = JsonSerializer.Serialize(onboarding);
                    await studentService.UpdateAsync(student);
                }
                catch { /* ignore parse errors */ }
            }

            return Created("/api/students/me/body-history", new LatestAssessmentDto
            {
                AssessmentDate = assessment.AssessmentDate,
                Weight = assessment.Weight,
                Height = assessment.Height,
                Bmi = assessment.Bmi,
                BodyFatPercentage = assessment.BodyFatPercentage,
                Measurements = string.IsNullOrEmpty(assessment.Measurements) ? null : assessment.Measurements,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpGet("me/body-history")]
    [SwaggerOperation(Summary = "Obter histórico de medições corporais do aluno autenticado")]
    [ProducesResponseType(typeof(IEnumerable<LatestAssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBodyHistory()
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var history = await assessmentRepository.GetAllAsync(a => a.StudentId == student.Id);
            var result = history
                .OrderByDescending(a => a.AssessmentDate)
                .Take(20)
                .Select(a => new LatestAssessmentDto
                {
                    AssessmentDate = a.AssessmentDate,
                    Weight = a.Weight,
                    Height = a.Height,
                    Bmi = a.Bmi,
                    BodyFatPercentage = a.BodyFatPercentage,
                    Measurements = string.IsNullOrEmpty(a.Measurements) ? null : a.Measurements,
                });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }

    [HttpPost("me/regenerate-workout")]
    [SwaggerOperation(Summary = "Gerar novo plano de treino baseado nos dias de treino escolhidos")]
    [ProducesResponseType(typeof(WorkoutSheetResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegenerateWorkout([FromBody] RegenerateWorkoutDto dto)
    {
        try
        {
            var userId = currentUserService.GetCurrentUserId();
            var student = await studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound(new ErrorResponse { Type = "NotFound", Message = "Aluno não encontrado.", StatusCode = 404 });

            var sheet = await workoutTemplateService.GenerateWorkoutAsync(student.Id, dto.TrainingDays);

            // Update training days in onboarding
            if (!string.IsNullOrEmpty(student.OnboardingDataJson))
            {
                try
                {
                    var onboarding = JsonSerializer.Deserialize<SaveOnboardingDto>(student.OnboardingDataJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SaveOnboardingDto();
                    onboarding.TrainingDays = dto.TrainingDays;
                    student.OnboardingDataJson = JsonSerializer.Serialize(onboarding);
                    await studentService.UpdateAsync(student);
                }
                catch { /* ignore parse errors */ }
            }

            return Created("/api/workouts/student/active", mapper.Map<WorkoutSheetResponseDto>(sheet));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Type = "InternalServerError", Message = ex.Message, StatusCode = 500 });
        }
    }
}
