using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : Controller
{
    private readonly EStoreDbContext _context;
    private readonly IProductRepository _productRepository;

    public CartController(EStoreDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cartJson = Request.Cookies["CartCookie"];

        if (cartJson == null) return Ok(new Cart());
        var cart = JsonConvert.DeserializeObject<Cart>(cartJson);
        return Ok(cart);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        var cartJson = Request.Cookies["CartCookie"];

        if (cartJson != null)
        {
            var cart = JsonConvert.DeserializeObject<Cart>(cartJson);
            var addingProduct = _productRepository.GetProductById(productId);
            if (addingProduct == null) return BadRequest("Product you want to add is not exist!");

            if (addingProduct.UnitInStock < quantity)
            {
                return BadRequest("Quantity you need out of stock!");
            }
            
            cart.AddItem(productId, quantity);

            Response.Cookies.Append("CartCookie", JsonConvert.SerializeObject(cart));
        }
        else
        {
            var cart = new Cart();
            cart.AddItem(productId, quantity);

            Response.Cookies.Append("CartCookie", JsonConvert.SerializeObject(cart));
        }

        return Ok();
    }

    [HttpDelete("{productId:int}")]
    public IActionResult RemoveFromCart(int productId)
    {
        var cartJson = Request.Cookies["CartCookie"];

        if (cartJson == null) return NotFound();
        var cart = JsonConvert.DeserializeObject<Cart>(cartJson);
        cart.RemoveItem(productId);

        Response.Cookies.Append("CartCookie", JsonConvert.SerializeObject(cart));

        return Ok();
    }
}