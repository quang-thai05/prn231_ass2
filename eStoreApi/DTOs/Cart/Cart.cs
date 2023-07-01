namespace Lab2.DTOs.Cart;

public class Cart
{
    public List<CartItem> Items { get; set; }

    public Cart()
    {
        Items = new List<CartItem>();
    }

    public void AddItem(int productId, int quantity)
    {
        var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var newItem = new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            };
            Items.Add(newItem);
        }
    }

    public void RemoveItem(int productId)
    {
        var itemToRemove = Items.FirstOrDefault(item => item.ProductId == productId);

        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
        }
    }
}