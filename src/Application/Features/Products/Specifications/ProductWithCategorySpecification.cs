
using Application.Features.Products.Queries;

namespace Application.Features.Products.Specifications;
public class ProductWithCategorySpecification : BaseSpecification<Product>
{
    // Constructor for pagination with your 5 parameters
    public ProductWithCategorySpecification(
      ProductSpecParams specParams)
        : base(x =>
            string.IsNullOrEmpty(specParams.SearchTerm) ||
            x.Name.ToLower().Contains(specParams.SearchTerm.ToLower())
        )
    {
        // Include Category
        AddInclude(x => x.Category);

        // Apply Sorting based on sortBy and sortDirection
        ApplySorting(specParams.SortBy, specParams.SortDirection);

        // Apply Pagination
        ApplyPaging(
            (specParams.PageNumber - 1) * specParams.PageSize,
            specParams.PageSize
        );
    }

    // Constructor for getting single product
    public ProductWithCategorySpecification(int id)
        : base(x => x.Id == id)
    {
        AddInclude(x => x.Category);
    }

    private void ApplySorting(string sortBy, string sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            AddOrderBy(x => x.Name);
            return;
        }

        var isDescending = sortDirection?.ToLower() == "desc";

        switch (sortBy.ToLower())
        {
            case "price":
                if (isDescending)
                    AddOrderByDescending(x => x.Price);
                else
                    AddOrderBy(x => x.Price);
                break;
            case "quantity":
                if (isDescending)
                    AddOrderByDescending(x => x.quantity);
                else
                    AddOrderBy(x => x.quantity);
                break;
            case "id":
                if (isDescending)
                    AddOrderByDescending(x => x.Id);
                else
                    AddOrderBy(x => x.Id);
                break;
            case "name":
            default:
                if (isDescending)
                    AddOrderByDescending(x => x.Name);
                else
                    AddOrderBy(x => x.Name);
                break;
        }
    }
}