using Microsoft.EntityFrameworkCore;
using Zyntra.Domain.Dtos.Pagination;

namespace Zyntra.Shared.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedListDto<T>> CreateAsync<T>(IQueryable<T> source, int pageNumber, int pageSize) where T : class
    {
        var count = await source.CountAsync();

        List<T> items;
        if (pageSize == -1)
        {
            items = await source.AsNoTracking().ToListAsync();
            return new PagedListDto<T>(items, 1, count, count);
        }

        items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        return new PagedListDto<T>(items, pageNumber, pageSize, count);
    }
}
