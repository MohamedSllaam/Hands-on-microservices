 
namespace Application.Features.Products.Queries;
 

 
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public int Id { get; set; }
}

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>()
            .GetByIdAsync(request.Id, x=>x.Category);

        if (product == null)
            throw new NotFoundException($"Product with ID {request.Id} not found");

       

        return _mapper.Map<ProductDto>(product);
    }
}