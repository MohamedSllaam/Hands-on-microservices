namespace Application.Features.Products.Commands;



public class DeleteProductCommand : ICommand<bool>
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
        if (product == null)
            throw new NotFoundException($"Product with ID {request.Id} not found");

        _unitOfWork.Repository<Product>().Delete(product);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}