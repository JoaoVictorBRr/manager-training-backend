using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.ChatDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class ChatService(IChatMessageRepository repo, IValidator<ChatMessage> validator) : IChatService
{
    private readonly IChatMessageRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<ChatMessage> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<ChatMessage> AddAsync(ChatMessage entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<ChatMessage> UpdateAsync(ChatMessage entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<ChatMessage> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<ChatMessage>> AddRangeListAsync(IList<ChatMessage> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<ChatMessage> entity) => _repo.UpdateRangeAsync(entity);
    public Task<ChatMessage> DeleteAsync(ChatMessage entity) => _repo.DeleteAsync(entity);
    public Task<ChatMessage> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<ChatMessage> entities) => _repo.DeleteRangeAsync(entities);
    public Task<ChatMessage> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<ChatMessage>> GetAllAsync(Expression<Func<ChatMessage, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<ChatMessage> GetAsync(Expression<Func<ChatMessage, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<IEnumerable<ChatMessage>> GetConversationAsync(long studentId, long instructorId)
        => _repo.GetConversationAsync(studentId, instructorId);

    public async Task<ChatMessage> SendMessageAsync(ChatMessageSendDto dto)
    {
        var message = new ChatMessage
        {
            StudentId = dto.StudentId,
            InstructorId = dto.InstructorId,
            Message = dto.Message,
            MessageDateTime = DateTime.Now,
            IsRead = false
        };

        return await this.AddAsync(message);
    }

    public async Task<List<ValidationFailure>> Validate(ChatMessage entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
