

namespace Application.Features.Products.Specifications;

public class ProductByNameSpecification : BaseSpecification<Product>
{
    public ProductByNameSpecification(string name)
        : base(x => x.Name.ToLower() == name.ToLower())
    {
    }
}
