namespace Shared.Pagination;


public class PaginatedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize); // This should calculate correctly
    public List<T> Items { get; set; } = new();
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public PaginatedResult(int pageNumber, int pageSize, int totalCount, List<T> items)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }
}