using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
// [Authorize]
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
    public IActionResult AddToCart([FromBody] ProductCartItem model)
    {
        var addingProduct = _productRepository.GetProductById(model.ProductId);
        if (addingProduct == null) return NotFound("Product you want to add is not exist!");

        var cartJson = Request.Cookies["CartCookie"];
        Cart cart;
        cart = cartJson != null ? JsonConvert.DeserializeObject<Cart>(cartJson) : new Cart();

        if (addingProduct.UnitInStock < model.Quantity)
        {
            return BadRequest("Quantity you need out of stock!");
        }

        cart.AddItem(model.ProductId, addingProduct.ProductName, model.Quantity, addingProduct.UnitPrice);
        addingProduct.UnitInStock -= model.Quantity;
        _context.SaveChanges();

        Response.Cookies.Append("CartCookie", JsonConvert.SerializeObject(cart));

        return Ok("Added successfully!");
    }

    [HttpDelete("{productId:int}")]
    public IActionResult RemoveFromCart(int productId)
    {
        var cartJson = Request.Cookies["CartCookie"];
        if (cartJson == null) return NotFound();
        var cart = JsonConvert.DeserializeObject<Cart>(cartJson);

        var removingProduct = _productRepository.GetProductById(productId);
        if (removingProduct == null) return NotFound("Product you want to add is not exist!");

        var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        var cartQuantity = cartItem.Quantity;

        cart.RemoveItem(productId);
        removingProduct.UnitInStock += cartQuantity;
        _context.SaveChanges();

        Response.Cookies.Append("CartCookie", JsonConvert.SerializeObject(cart));

        return Ok();
    }
}