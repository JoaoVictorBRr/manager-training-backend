namespace Zyntra.Domain.Dtos.Pagination;

public class PagedListDto<T> : List<T>
{
    public int Start { get; set; }
    public int Take { get; set; } = 10;
    public int Draw { get; set; }
    public int TotalCount { get; set; }

    public PagedListDto(IEnumerable<T> items, int start, int take)
    {
        AddRange(items);
        Start = start;
        Take = take;
    }

    public PagedListDto(IEnumerable<T> items, int start, int take, int totalCount)
    {
        AddRange(items);
        Start = start;
        Take = take;
        TotalCount = totalCount;
    }
}
