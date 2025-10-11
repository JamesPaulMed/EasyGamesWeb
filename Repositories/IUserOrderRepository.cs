namespace EasyGamesWeb.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);

        Task changeOrderStat(OrderStatusUpdateModel data);

        Task togglePayStatus(int orderId);

        Task<Order?> getOrderbyId(int id);

        Task<IEnumerable<OrderStat>> getOrderStats();
    }
}