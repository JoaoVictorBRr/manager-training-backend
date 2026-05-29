using System.Text.Json;

namespace Zyntra.Domain.Dtos.AiChatDto;

public class AiChatActionDto
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public JsonElement Params { get; set; }
}
