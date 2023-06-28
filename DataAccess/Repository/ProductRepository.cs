using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly EStoreDbContext _context;

    public ProductRepository(EStoreDbContext context) : base(context)
    {
        _context = context;
    }

    public Product? GetProductById(int id)
    {
        var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
        return product;
    }

    public List<Product>? SearchProduct(string keyword)
    {
        List<Product> products;
        if (keyword.All(char.IsDigit))
        {
            products = _context.Products
                .Where(x => x.ProductName.ToLower().Contains(keyword.ToLower()) ||
                            x.UnitPrice == decimal.Parse(keyword))
                .Include(x => x.Category)
                .ToList();
        }
        else
        {
            products = _context.Products
                .Where(x => x.ProductName.ToLower().Contains(keyword.ToLower()))
                .Include(x => x.Category)
                .ToList();
        }

        return products;
    }
}