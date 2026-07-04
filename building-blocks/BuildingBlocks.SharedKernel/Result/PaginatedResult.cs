namespace BuildingBlocks.SharedKernel.Result;

public sealed class PaginatedResult<TDto>
(int pageIndex, int pageSize, long count, IEnumerable<TDto> data)
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public IEnumerable<TDto> Data { get; } = data;
}
