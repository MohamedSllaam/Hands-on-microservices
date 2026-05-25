

namespace Application.Features.Products.Specifications;


using global::Application.Features.Products.Queries;


public class ProductsWithCountSpecification : BaseSpecification<Product>
{
    public ProductsWithCountSpecification(ProductSpecParams specParams)
        : base(x =>
            string.IsNullOrEmpty(specParams.SearchTerm) ||
            x.Name.ToLower().Contains(specParams.SearchTerm)
        )
    {

    }
}