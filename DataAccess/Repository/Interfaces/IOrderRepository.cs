using BussinessObject;

namespace DataAccess.Repository.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    List<Order> GetOrdersByUserId(string userId);

    Order? GetById(int id);
}