using SharedModels.Models;

namespace OrderService.Services;

public interface IOrderService
{
    IEnumerable<Order> GetAllOrders();
    Order GetOrderById(int id);
    void AddOrder(Order order);
}
