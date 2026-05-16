namespace Zyntra.Shared.Models;

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public int StatusCode { get; set; }
}
