using Microsoft.AspNetCore.Mvc;
using ProductService.Services;
using SharedModels.Models;

namespace ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Product product)
    {
        _productService.AddProduct(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}
