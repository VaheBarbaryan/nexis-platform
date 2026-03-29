namespace SharedKernel.Application.Pagination;

public sealed record CursorResponse<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasNextPage);

public static class CursorResponse
{
    public static CursorResponse<TResult> From<TSource, TResult>(
        IReadOnlyList<TSource> rawItems,
        int limit,
        Func<TSource, TResult> map,
        Func<TSource, string> encodeCursor)
    {
        ArgumentNullException.ThrowIfNull(rawItems);
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(encodeCursor);

        var hasNextPage = rawItems.Count > limit;
        var items = hasNextPage ? rawItems.Take(limit).ToList() : rawItems;

        return new CursorResponse<TResult>(
            items.Select(map).ToList(),
            hasNextPage ? encodeCursor(items[^1]) : null,
            hasNextPage);
    }
}
