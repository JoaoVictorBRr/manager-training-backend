using Newtonsoft.Json;

namespace Atomo.Hub.Crm.Domain.Dtos.Pagination;

public class PaginatedResponse<T>
{
    [JsonProperty("page")]
    public int PageNumber { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }

    [JsonProperty("total")]
    public int TotalCount { get; set; }

    [JsonProperty("totalPages")]
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 1;

    [JsonProperty("hasNextPage")]
    public bool HasNextPage => PageNumber < TotalPages;

    [JsonProperty("hasPreviousPage")]
    public bool HasPreviousPage => PageNumber > 1;

    [JsonProperty("data")]
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
