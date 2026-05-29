using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class AiChatMessage : EntityBase
{
    public long StudentId { get; set; }
    public string Role { get; set; } = "user"; // "user" | "assistant"
    public string Content { get; set; } = string.Empty;
    public string? ActionJson { get; set; }
    public string ActionStatus { get; set; } = "none"; // "none" | "pending" | "confirmed" | "rejected"

    public Student Student { get; set; } = null!;
}
