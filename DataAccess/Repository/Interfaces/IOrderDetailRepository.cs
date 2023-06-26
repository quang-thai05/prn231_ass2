using BussinessObject;

namespace DataAccess.Repository.Interfaces;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    public List<OrderDetail>? GetByOrderId(int orderId);

    public List<OrderDetail>? GetByProductId(int productId);

    public Task AddOrderDetails(IEnumerable<OrderDetail> orderDetails);

    public Task DeleteOrderDetails(IEnumerable<OrderDetail> orderDetails);
}