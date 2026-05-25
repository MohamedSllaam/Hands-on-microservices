

namespace Application.Features.Products.DTOs;

// MyApp.Application/Features/Products/DTOs/ProductDtos.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
}

public class CreateUpdateProductDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}

 