using Lab2.DTOs.Cart;

namespace Lab2.Controllers;

public class CartService
{
    private readonly Dictionary<string, List<CartItem>> _carts;

    public CartService()
    {
        _carts = new Dictionary<string, List<CartItem>>();
    }

    public List<CartItem> GetCartItems(string token)
    {
        if (_carts.ContainsKey(token))
        {
            return _carts[token];
        }

        return new List<CartItem>();
    }
    
    public CartItem GetCartItem(string token, int productId)
    {
        if (_carts.ContainsKey(token))
        {
            var cartItems = _carts[token];
            var item = cartItems.FirstOrDefault(item => item.ProductId == productId);
            return item;
        }

        return null;
    }

    public void AddCartItem(string token, CartItem cartItem)
    {
        if (!_carts.ContainsKey(token))
        {
            _carts[token] = new List<CartItem>();
        }

        var existingItem = _carts[token].FirstOrDefault(item => item.ProductId == cartItem.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += cartItem.Quantity;
        }
        else
        {
            _carts[token].Add(cartItem);
        }
    }

    public void RemoveCartItem(string token, int productId)
    {
        if (_carts.ContainsKey(token))
        {
            var cartItems = _carts[token];
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }
        }
    }

    public void RemoveAllItem(string token)
    {
        if (_carts.ContainsKey(token))
        {
            _carts.Remove(token);
        }
    }
}