using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : Controller
{
    private readonly IProductRepository _repository;
    private readonly EStoreDbContext _context;

    public ProductController(IProductRepository repository, EStoreDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet]
    public ActionResult GetAllProducts()
    {
        try
        {
            var products = _context.Products
                .Include(x => x.Category)
                .Select(x => new
                {
                    ProductId = x.ProductId,
                    Category = x.Category.CategoryName,
                    ProductName = x.ProductName,
                    Weight = x.Weight,
                    UnitPrice = x.UnitPrice,
                    UnitInStock = x.UnitInStock
                })
                .ToList();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult GetProductById(int id)
    {
        try
        {
            var product = _repository.GetProductById(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public ActionResult SearchProduct(string keyword)
    {
        try
        {
            var products = _repository.SearchProduct(keyword);
            var result = products.Select(x => new
            {
                ProductId = x.ProductId,
                Category = x.Category.CategoryName,
                ProductName = x.ProductName,
                Weight = x.Weight,
                UnitPrice = x.UnitPrice,
                UnitInStock = x.UnitInStock
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddProduct([FromBody] ProductDto productDto)
    {
        try
        {
            var product = new Product()
            {
                CategoryId = productDto.CategoryId,
                ProductName = productDto.ProductName,
                Weight = productDto.Weight,
                UnitPrice = productDto.UnitPrice,
                UnitInStock = productDto.UnitInStock
            };
            await _repository.Add(product);
            return Ok("Product Added Successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductDto model)
    {
        try
        {
            var product = _repository.GetProductById(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.ProductName = model.ProductName;
            product.CategoryId = model.CategoryId;
            product.UnitPrice = model.UnitPrice;
            product.UnitInStock = model.UnitInStock;
            await _repository.Update(product);
            return Ok("Product Updated Successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = _repository.GetProductById(id);
            if (product is null)
            {
                return NotFound();
            }

            await _repository.Delete(product);
            return Ok("Deleted successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}