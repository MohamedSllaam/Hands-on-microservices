namespace Application.Features.Products.Queries;

public class ProductSpecParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    private string _searchTerm = string.Empty;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => _searchTerm = value?.ToLower() ?? string.Empty;
    }

    public string SortBy { get; set; } = "name";

    public string SortDirection { get; set; } = "asc";
}