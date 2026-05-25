namespace Application.Features.Products.Commands;


public class UpdateProductCommand : ICommand<ProductDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid product ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid category is required");
    }
}

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Get existing product
        var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
        if (existingProduct == null)
            throw new NotFoundException($"Product with ID {request.Id} not found");

        // Check if category exists
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException($"Category with ID {request.CategoryId} not found");

        // Update product
        _mapper.Map(request, existingProduct);
        _unitOfWork.Repository<Product>().Update(existingProduct);
        await _unitOfWork.SaveChangesAsync();

        // Get updated product with category
        var spec = new ProductWithCategorySpecification(request.Id);
        var updatedProduct = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

        return _mapper.Map<ProductDto>(updatedProduct);
    }
}