using BussinessObject;

namespace DataAccess.Repository.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Product? GetProductById(int id);

    List<Product>? SearchProduct(string keyword);
}