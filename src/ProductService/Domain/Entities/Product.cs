
namespace Domain.Entities;
 
public class Product : BaseEntity
{

    public string Name { get; set; } = null!;
    public decimal Price { get; set; } 
    public int quantity { get; set; }
    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
   
}
