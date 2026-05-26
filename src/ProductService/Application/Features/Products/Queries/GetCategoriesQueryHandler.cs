namespace Application.Features.Products.Queries;


public class GetCategoriesQuery : IQuery<List<CategoryDto>>
{
}

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<Category>()
            .GetAllAsync(cancellationToken);

        return _mapper.Map<List<CategoryDto>>(categories);
    }
}