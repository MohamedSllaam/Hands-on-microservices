using Microsoft.AspNetCore.Mvc;
using Application.Features.Products.Commands;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Shared.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products with pagination, filtering, and sorting
    /// </summary>
     
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
     [FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductCommand command)
    {
        
        command.Id=id;
       var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("Categories")]
    public async Task<ActionResult<ProductDto>> GetCategories()
    {
        var query = new GetCategoriesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}