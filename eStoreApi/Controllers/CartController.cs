using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class CartController : Controller
{
    private readonly EStoreDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly CartService _cartService;

    public CartController(EStoreDbContext context, IProductRepository productRepository, CartService cartService)
    {
        _context = context;
        _productRepository = productRepository;
        _cartService = cartService;
    }

    [HttpGet]
    public ActionResult<List<CartItem>> GetCartItems()
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"];
            if (token == null) return Unauthorized();
            var cartItems = _cartService.GetCartItems(token);
            return cartItems;
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult AddCartItem([FromBody] ProductCartItem model)
    {
        try
        {
            var addingProduct = _context.Products.FirstOrDefault(x => x.ProductId.Equals(model.ProductId));
            if (addingProduct is null) return NotFound("Product is not exist");
            if (model.Quantity > addingProduct.UnitInStock) return BadRequest("Do not have enough items in stock");

            string token = HttpContext.Request.Headers["Authorization"];
            if (token == null) return Unauthorized();

            var cartItem = new CartItem()
            {
                ProductId = model.ProductId,
                ProductName = addingProduct.ProductName,
                Quantity = model.Quantity,
                UnitPrice = addingProduct.UnitPrice
            };
            addingProduct.UnitInStock -= model.Quantity;
            _context.SaveChanges();
            _cartService.AddCartItem(token, cartItem);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{productId:int}")]
    public IActionResult RemoveCartItem(int productId)
    {
        try
        {
            var removeItem = _context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (removeItem is null) return NotFound();
            
            string token = HttpContext.Request.Headers["Authorization"];
            if (token == null) return Unauthorized();
            
            var item = _cartService.GetCartItem(token, productId);
            if (item != null)
            {
                removeItem.UnitInStock += item.Quantity;
                _context.SaveChanges();
            }
            _cartService.RemoveCartItem(token, productId);
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete]
    public ActionResult RemoveAllItems()
    {
        try
        {
            string token = HttpContext.Request.Headers["Authorization"];
            if (token == null) return Unauthorized();
            
            _cartService.RemoveAllItem(token);
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}