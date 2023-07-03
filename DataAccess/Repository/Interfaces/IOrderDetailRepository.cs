using BussinessObject;

namespace DataAccess.Repository.Interfaces;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    public List<OrderDetail>? GetByOrderId(int orderId);

    public List<OrderDetail>? GetByProductId(int productId);

    public Task AddOrderDetails(int orderId, List<OrderDetail> orderDetails);

    public Task DeleteOrderDetails(int id);
}