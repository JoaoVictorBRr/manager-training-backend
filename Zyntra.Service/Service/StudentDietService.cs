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

    public Task<StudentDietResponseDto> GenerateDefaultDietAsync(long studentId, string? objective, List<string>? dietRestrictions)
    {
        var isVeg = dietRestrictions?.Any(r =>
            r.Contains("vegetarian", StringComparison.OrdinalIgnoreCase) ||
            r.Contains("vegano", StringComparison.OrdinalIgnoreCase) ||
            r.Contains("vegan", StringComparison.OrdinalIgnoreCase)) ?? false;

        var dto = objective switch
        {
            "lose_weight" => BuildLoseWeightDiet(isVeg),
            "gain_muscle" => BuildGainMuscleDiet(isVeg),
            _             => BuildBalancedDiet(isVeg),
        };

        return CreateDietAsync(studentId, dto);
    }

    private static CreateDietDto BuildLoseWeightDiet(bool veg) => new()
    {
        Name = "Plano para Emagrecimento",
        Meals =
        [
            new() { MealType = MealType.Breakfast, Options =
            [
                new() { FoodName = "Ovos mexidos", Quantity = "2 unidades", Calories = 140, Protein = 12, Carbs = 2, Fat = 9 },
                new() { FoodName = "Pão integral", Quantity = "1 fatia", Calories = 70, Protein = 3, Carbs = 13, Fat = 1 },
            ]},
            new() { MealType = MealType.Lunch, Options =
            [
                new() { FoodName = veg ? "Tofu grelhado" : "Frango grelhado", Quantity = "150g", Calories = 160, Protein = veg ? 16 : 32, Carbs = veg ? 4 : 0, Fat = 5 },
                new() { FoodName = "Arroz integral", Quantity = "4 colheres de sopa", Calories = 140, Protein = 3, Carbs = 30, Fat = 1 },
                new() { FoodName = "Salada verde", Quantity = "à vontade", Calories = 30, Protein = 2, Carbs = 5, Fat = 0 },
            ]},
            new() { MealType = MealType.AfternoonSnack, Options =
            [
                new() { FoodName = "Iogurte grego natural", Quantity = "170g", Calories = 100, Protein = 17, Carbs = 6, Fat = 0 },
                new() { FoodName = "Maçã", Quantity = "1 unidade", Calories = 80, Protein = 0, Carbs = 21, Fat = 0 },
            ]},
            new() { MealType = MealType.Dinner, Options =
            [
                new() { FoodName = veg ? "Lentilha cozida" : "Peixe grelhado", Quantity = "150g", Calories = 160, Protein = veg ? 13 : 30, Carbs = veg ? 28 : 0, Fat = 2 },
                new() { FoodName = "Batata doce", Quantity = "100g", Calories = 90, Protein = 2, Carbs = 21, Fat = 0 },
                new() { FoodName = "Brócolis cozido", Quantity = "100g", Calories = 35, Protein = 3, Carbs = 7, Fat = 0 },
            ]},
        ],
    };

    private static CreateDietDto BuildGainMuscleDiet(bool veg) => new()
    {
        Name = "Plano para Ganho de Massa",
        Meals =
        [
            new() { MealType = MealType.Breakfast, Options =
            [
                new() { FoodName = "Ovos mexidos", Quantity = "3 unidades", Calories = 210, Protein = 18, Carbs = 3, Fat = 14 },
                new() { FoodName = "Pão integral", Quantity = "2 fatias", Calories = 140, Protein = 6, Carbs = 26, Fat = 2 },
                new() { FoodName = "Banana", Quantity = "1 unidade", Calories = 90, Protein = 1, Carbs = 23, Fat = 0 },
            ]},
            new() { MealType = MealType.Lunch, Options =
            [
                new() { FoodName = veg ? "Grão-de-bico cozido" : "Frango grelhado", Quantity = "200g", Calories = veg ? 240 : 220, Protein = veg ? 14 : 42, Carbs = veg ? 40 : 0, Fat = veg ? 4 : 5 },
                new() { FoodName = "Arroz integral", Quantity = "6 colheres de sopa", Calories = 210, Protein = 4, Carbs = 45, Fat = 1 },
                new() { FoodName = "Feijão cozido", Quantity = "1 concha", Calories = 140, Protein = 9, Carbs = 25, Fat = 1 },
            ]},
            new() { MealType = MealType.AfternoonSnack, Options =
            [
                new() { FoodName = "Batata doce", Quantity = "150g", Calories = 135, Protein = 3, Carbs = 31, Fat = 0 },
                new() { FoodName = veg ? "Iogurte grego" : "Frango grelhado", Quantity = veg ? "200g" : "100g", Calories = veg ? 120 : 110, Protein = veg ? 20 : 21, Carbs = veg ? 7 : 0, Fat = veg ? 0 : 2 },
            ]},
            new() { MealType = MealType.Dinner, Options =
            [
                new() { FoodName = veg ? "Tofu grelhado" : "Carne vermelha magra", Quantity = "200g", Calories = veg ? 160 : 260, Protein = veg ? 21 : 42, Carbs = veg ? 4 : 0, Fat = veg ? 8 : 10 },
                new() { FoodName = "Arroz integral", Quantity = "4 colheres de sopa", Calories = 140, Protein = 3, Carbs = 30, Fat = 1 },
                new() { FoodName = "Legumes salteados", Quantity = "100g", Calories = 60, Protein = 2, Carbs = 12, Fat = 1 },
            ]},
            new() { MealType = MealType.Supper, Options =
            [
                new() { FoodName = "Queijo cottage", Quantity = "200g", Calories = 140, Protein = 24, Carbs = 8, Fat = 2 },
                new() { FoodName = "Castanhas", Quantity = "20g", Calories = 130, Protein = 3, Carbs = 3, Fat = 12 },
            ]},
        ],
    };

    private static CreateDietDto BuildBalancedDiet(bool veg) => new()
    {
        Name = "Plano Alimentar Equilibrado",
        Meals =
        [
            new() { MealType = MealType.Breakfast, Options =
            [
                new() { FoodName = "Ovos mexidos", Quantity = "2 unidades", Calories = 140, Protein = 12, Carbs = 2, Fat = 9 },
                new() { FoodName = "Pão integral", Quantity = "2 fatias", Calories = 140, Protein = 6, Carbs = 26, Fat = 2 },
                new() { FoodName = "Fruta da estação", Quantity = "1 unidade média", Calories = 70, Protein = 1, Carbs = 18, Fat = 0 },
            ]},
            new() { MealType = MealType.Lunch, Options =
            [
                new() { FoodName = veg ? "Feijão cozido" : "Frango grelhado", Quantity = veg ? "2 conchas" : "180g", Calories = veg ? 280 : 200, Protein = veg ? 18 : 38, Carbs = veg ? 50 : 0, Fat = veg ? 2 : 4 },
                new() { FoodName = "Arroz integral", Quantity = "5 colheres de sopa", Calories = 175, Protein = 4, Carbs = 37, Fat = 1 },
                new() { FoodName = "Salada variada", Quantity = "à vontade", Calories = 40, Protein = 2, Carbs = 8, Fat = 0 },
            ]},
            new() { MealType = MealType.AfternoonSnack, Options =
            [
                new() { FoodName = "Iogurte grego natural", Quantity = "170g", Calories = 100, Protein = 17, Carbs = 6, Fat = 0 },
                new() { FoodName = "Castanhas", Quantity = "30g", Calories = 195, Protein = 4, Carbs = 4, Fat = 19 },
            ]},
            new() { MealType = MealType.Dinner, Options =
            [
                new() { FoodName = veg ? "Omelete de legumes" : "Omelete", Quantity = "3 ovos", Calories = 210, Protein = 18, Carbs = veg ? 8 : 3, Fat = 14 },
                new() { FoodName = "Batata doce", Quantity = "100g", Calories = 90, Protein = 2, Carbs = 21, Fat = 0 },
                new() { FoodName = "Legumes no vapor", Quantity = "100g", Calories = 50, Protein = 3, Carbs = 10, Fat = 0 },
            ]},
        ],
    };

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
