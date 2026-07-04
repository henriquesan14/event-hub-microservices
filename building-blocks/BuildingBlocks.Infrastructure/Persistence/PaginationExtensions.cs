using BuildingBlocks.SharedKernel.Result;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.SharedKernel.Pagination;

public static class PaginationExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<T>(
            page,
            pageSize,
            totalCount,
            items);
    }
}
