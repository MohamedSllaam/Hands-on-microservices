

namespace Application.Features.Products.Commands;

 
// MyApp.Application/Features/Products/Commands/CreateProductCommand.cs
using Application.Features.Products.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
 using global::Application.Features.Products.Specifications;
using Shared.CQRS;
using Shared.Exceptions;

public class CreateProductCommand : ICommand<ProductDto>
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .PrecisionScale(18, 2, true).WithMessage("Price must have up to 2 decimal places");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid category is required");
    }
}

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check if category exists
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException($"Category with ID {request.CategoryId} not found");


        var spec = new ProductByNameSpecification(request.Name);
        // Check if product with same name already exists (optional)
        var existingProduct = await _unitOfWork.Repository<Product>()
            .FindOneWithSpecificationAsync(spec);

        if (existingProduct != null)
            throw new ValidationException($"Product with name '{request.Name}' already exists");

        // Create new product
        var product = _mapper.Map<Product>(request);

         _unitOfWork.Repository<Product>().Add(product);
        await _unitOfWork.SaveChangesAsync();

        // Get created product with category
        var createdProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(product.Id, x=>x.Category);

        return _mapper.Map<ProductDto>(createdProduct);
    }
}