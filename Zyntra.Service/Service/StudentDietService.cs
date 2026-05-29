using System.Linq.Expressions;
using FluentValidation.Results;
using Zyntra.Domain.Dtos.DietDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class StudentDietService(
    IStudentDietRepository dietRepo,
    IDietMealRepository mealRepo,
    IDietMealOptionRepository optionRepo,
    IDietMealPhotoRepository photoRepo) : IStudentDietService
{
    public async Task<StudentDietResponseDto?> GetActiveDietAsync(long studentId)
    {
        var diet = await dietRepo.GetActiveDietByStudentAsync(studentId);
        if (diet == null) return null;

        return MapToDto(diet);
    }

    public async Task<StudentDietResponseDto> CreateDietAsync(long studentId, CreateDietDto dto)
    {
        // Deactivate current active diet
        var existing = await dietRepo.GetActiveDietByStudentAsync(studentId);
        if (existing != null)
        {
            existing.IsActive = false;
            await dietRepo.UpdateAsync(existing);
        }

        var diet = new StudentDiet
        {
            StudentId = studentId,
            Name = dto.Name,
            IsActive = true,
            GeneratedAt = DateTime.Now,
        };
        await dietRepo.AddAsync(diet);

        foreach (var mealDto in dto.Meals)
        {
            var meal = new DietMeal
            {
                StudentDietId = diet.Id,
                MealType = mealDto.MealType,
                IsCompleted = false,
            };
            await mealRepo.AddAsync(meal);

            foreach (var optDto in mealDto.Options)
            {
                var option = new DietMealOption
                {
                    DietMealId = meal.Id,
                    FoodName = optDto.FoodName,
                    Quantity = optDto.Quantity,
                    Calories = optDto.Calories,
                    Protein = optDto.Protein,
                    Carbs = optDto.Carbs,
                    Fat = optDto.Fat,
                    Ingredients = optDto.Ingredients,
                    PreparationMethod = optDto.PreparationMethod,
                };
                await optionRepo.AddAsync(option);
            }
        }

        var created = await dietRepo.GetActiveDietByStudentAsync(studentId);
        return MapToDto(created!);
    }

    public async Task CompleteMealAsync(long dietMealId)
    {
        var meal = await mealRepo.GetByIdAsync(dietMealId);
        if (meal == null) return;

        meal.IsCompleted = true;
        meal.CompletedAt = DateTime.Now;
        await mealRepo.UpdateAsync(meal);
    }

    public async Task AddMealPhotoAsync(long dietMealOptionId, string imagePath)
    {
        var photo = new DietMealPhoto
        {
            DietMealOptionId = dietMealOptionId,
            ImagePath = imagePath,
            UploadedAt = DateTime.Now,
        };
        await photoRepo.AddAsync(photo);
    }

    private static StudentDietResponseDto MapToDto(StudentDiet diet)
    {
        var meals = diet.Meals.Select(m => new DietMealDto
        {
            Id = m.Id,
            MealType = m.MealType,
            MealTypeName = GetMealTypeName(m.MealType),
            IsCompleted = m.IsCompleted,
            CompletedAt = m.CompletedAt,
            Options = m.Options.Select(o => new DietMealOptionDto
            {
                Id = o.Id,
                FoodName = o.FoodName,
                Quantity = o.Quantity,
                Calories = o.Calories,
                Protein = o.Protein,
                Carbs = o.Carbs,
                Fat = o.Fat,
                Ingredients = o.Ingredients,
                PreparationMethod = o.PreparationMethod,
                PhotoUrls = o.Photos.Select(p => p.ImagePath).ToList(),
            }).ToList(),
        }).OrderBy(m => m.MealType).ToList();

        var completedMeals = meals.Where(m => m.IsCompleted).ToList();
        var totalCal = meals.SelectMany(m => m.Options).Sum(o => o.Calories);
        var consumedCal = completedMeals.SelectMany(m => m.Options).Sum(o => o.Calories);

        return new StudentDietResponseDto
        {
            Id = diet.Id,
            Name = diet.Name,
            IsActive = diet.IsActive,
            GeneratedAt = diet.GeneratedAt,
            Meals = meals,
            TotalCalories = totalCal,
            ConsumedCalories = consumedCal,
        };
    }

    private static string GetMealTypeName(MealType type) => type switch
    {
        MealType.Breakfast => "Café da Manhã",
        MealType.Lunch => "Almoço",
        MealType.AfternoonSnack => "Café da Tarde",
        MealType.Dinner => "Janta",
        MealType.Supper => "Ceia",
        _ => type.ToString(),
    };

    // IServiceBase boilerplate
    public Task<StudentDiet> AddAsync(StudentDiet entity) => dietRepo.AddAsync(entity);
    public Task AddRangeAsync(IList<StudentDiet> entity) => dietRepo.AddRangeAsync(entity);
    public Task<IList<StudentDiet>> AddRangeListAsync(IList<StudentDiet> entities) => dietRepo.AddRangeListAsync(entities);
    public Task<StudentDiet> UpdateAsync(StudentDiet entity) => dietRepo.UpdateAsync(entity);
    public Task UpdateRangeAsync(IEnumerable<StudentDiet> entity) => dietRepo.UpdateRangeAsync(entity);
    public Task<StudentDiet> DeleteAsync(StudentDiet entity) => dietRepo.DeleteAsync(entity);
    public Task<StudentDiet> DeleteAsync(long Id) => dietRepo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<StudentDiet> entities) => dietRepo.DeleteRangeAsync(entities);
    public Task<StudentDiet> GetByIdAsync(long Id) => dietRepo.GetByIdAsync(Id);
    public Task<IEnumerable<StudentDiet>> GetAllAsync(Expression<Func<StudentDiet, bool>> predicate) => dietRepo.GetAllAsync(predicate);
    public Task<StudentDiet> GetAsync(Expression<Func<StudentDiet, bool>> predicate) => dietRepo.GetAsync(predicate);
    public Task<List<ValidationFailure>> Validate(StudentDiet entity) => Task.FromResult(new List<ValidationFailure>());
}
