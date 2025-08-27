using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using SharedModels;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_orderService.GetAllOrders());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var order = _orderService.GetOrderById(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Order order)
    {
        _orderService.AddOrder(order);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}
