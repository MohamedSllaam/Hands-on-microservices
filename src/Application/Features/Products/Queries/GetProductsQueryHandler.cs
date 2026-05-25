
namespace Application.Features.Products.Queries;

// MyApp.Application/Features/Products/Queries/GetProductsQuery.cs

using Domain.Entities;
using Domain.Interfaces;
using global::Application.Features.Products.Specifications;
using Shared.CQRS;
using Shared.Pagination;


public class GetProductsQuery : IQuery<PaginatedResult<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; } = string.Empty;
    public string SortBy { get; set; } = "id";
    public string SortDirection { get; set; } = "asc";
}

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PaginatedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var specParams = new ProductSpecParams
        {
               SortBy = request.SortBy,
               SearchTerm = request.SearchTerm,    
               PageSize = request.PageSize,
              PageNumber = request.PageNumber,
              SortDirection = request.SortDirection,
                      
        };



        var spec = new ProductWithCategorySpecification(specParams);

        // Create count specification (same filters but no pagination)


        // Get products with specification
        var products = await _unitOfWork.Repository<Product>().FindWithSpecificationAsync(spec);


        var countSpec = new ProductsWithCountSpecification(specParams);


       
        var totalCount = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

        


        // Make sure pageSize and pageNumber are correct
        return new PaginatedResult<ProductDto>(
            request.PageNumber,  // Use request.PageNumber, not specParams.PageNumber
            request.PageSize,    // Use request.PageSize, not specParams.PageSize
            totalCount,
            _mapper.Map<List<ProductDto>>(products)
        );
    }
}