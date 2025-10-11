namespace EasyGamesWeb.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int productId, int qty);
        Task<int> RemoveItem(int productId);
        Task<Cart> getUserCart();

        Task<Cart> getCart(string userId);
        Task<int> getCartItemCount(string userId = "");

        Task<bool> StartCheckout(CheckoutModel model);

    } 
}
