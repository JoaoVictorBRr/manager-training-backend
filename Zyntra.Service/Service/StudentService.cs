using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using System.Text.Json;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class StudentService(IStudentRepository repo, IValidator<Student> validator, IPhysicalAssessmentRepository assessmentRepo) : IStudentService
{
    private readonly IStudentRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Student> _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IPhysicalAssessmentRepository _assessmentRepo = assessmentRepo ?? throw new ArgumentNullException(nameof(assessmentRepo));

    public async Task<Student> AddAsync(Student entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Student> UpdateAsync(Student entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Student> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Student>> AddRangeListAsync(IList<Student> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Student> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Student> DeleteAsync(Student entity) => _repo.DeleteAsync(entity);
    public Task<Student> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Student> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Student> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Student> GetAsync(Expression<Func<Student, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter)
        => _repo.FilterAllStudents(filter);

    public async Task<Student?> GetByUserIdAsync(long userId)
        => await _repo.GetAsync(s => s.UserId == userId);

    public async Task<Student> SaveOnboardingAsync(long userId, SaveOnboardingDto dto)
    {
        var student = await _repo.GetAsync(s => s.UserId == userId)
            ?? throw new InvalidOperationException("Perfil de aluno não encontrado.");

        student.Objective = dto.Objective;
        student.OnboardingDataJson = dto.OnboardingDataJson ?? JsonSerializer.Serialize(dto);
        student.OnboardingCompleted = true;

        await _repo.UpdateAsync(student);

        if (dto.Weight.HasValue && dto.Height.HasValue)
        {
            var heightM = dto.Height.Value > 10 ? dto.Height.Value / 100m : dto.Height.Value;
            var bmi = dto.Weight.Value / (heightM * heightM);

            await _assessmentRepo.AddAsync(new PhysicalAssessment
            {
                StudentId = student.Id,
                AssessmentDate = DateTime.UtcNow,
                Weight = dto.Weight.Value,
                Height = heightM,
                Bmi = Math.Round(bmi, 2),
                Notes = "Avaliação inicial — dados do onboarding"
            });
        }

        return student;
    }

    public async Task<List<ValidationFailure>> Validate(Student entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
