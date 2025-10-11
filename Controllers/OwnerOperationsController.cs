using EasyGamesWeb.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace EasyGamesWeb.Controllers;

[Authorize (Roles = nameof (Roles.Admin))]
public class OwnerOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    public OwnerOperationsController(IUserOrderRepository userOrderRepository)
    {
        _userOrderRepository = userOrderRepository;

    }
    public async Task<IActionResult> allOrders()
     {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders); 
    }

    public async Task <IActionResult> togglePayStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.togglePayStatus(orderId);
        }
        catch (Exception ex)
        {
            //This is where to log exception
        }

        return RedirectToAction(nameof(allOrders)) ;
    }

    public async Task<IActionResult> updateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.getOrderbyId(orderId);
        if (order == null) {
            throw new InvalidOperationException($"Order with the ID:{orderId} was not found");
        }
        var orderStatList = (await _userOrderRepository.getOrderStats()).Select(orderStat =>
        {
            return new SelectListItem { Value = orderStat.Id.ToString(), Text = orderStat.StatName, Selected = order.OrderStatId == orderStat.Id };
        });
        var data = new OrderStatusUpdateModel
        {
            OrderId = orderId,
            OrderStatId = order.OrderStatId,
            OrderStatList = orderStatList
        };
        return View(data);
            }
    [HttpPost]
    public async Task <IActionResult> updateOrderStatus
        (OrderStatusUpdateModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatList = (await _userOrderRepository.getOrderStats()).Select(orderStat =>
                {
                    return new SelectListItem
                    {
                        Value =
                    orderStat.Id.ToString(),
                        Text = orderStat.StatName,
                        Selected = orderStat.Id == data.OrderStatId
                    };
                });
                return View(data);
            }
            await _userOrderRepository.changeOrderStat(data);
            TempData["msg"] = "Updated Successfully";
        }

        catch (Exception ex)
        {
            TempData["msg"] = "Update Failed";
        }
        return RedirectToAction(nameof(updateOrderStatus), new { orderId = data.OrderId });
    }
}
