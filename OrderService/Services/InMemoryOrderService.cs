using SharedModels.Models;

namespace OrderService.Services;

public class InMemoryOrderService : IOrderService
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public IEnumerable<Order> GetAllOrders() => _orders;

    public Order GetOrderById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public void AddOrder(Order order)
    {
        order.Id = _nextId++;
        order.OrderDate = DateTime.UtcNow;
        _orders.Add(order);
    }
}
