using FluentValidation.Results;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class ClassService(IClassRepository classRepository) : BaseService<Class>(classRepository), IClassService
{
    private readonly IClassRepository _classRepository = classRepository;

    public Task<PagedListDto<Class>> FilterAllClasses(ClassRequestListDto filter)
        => _classRepository.FilterAllClasses(filter);

    public Task<IEnumerable<Class>> GetAvailableClassesAsync()
        => _classRepository.GetAvailableClassesAsync();

    public Task<List<ValidationFailure>> Validate(Class entity)
        => Task.FromResult(new List<ValidationFailure>());
}
