namespace Application.Features.Products.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {

        CreateMap<Category, CategoryDto>();
        // Entity to DTO
        CreateMap<Product, ProductDto>()
            
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.CategoryName,
                       opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Quantity,
                       opt => opt.MapFrom(src => src.quantity));

        // Command to Entity
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.quantity,
                       opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Category,
                       opt => opt.Ignore());

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.quantity,
                       opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Category,
                       opt => opt.Ignore());
    }
}