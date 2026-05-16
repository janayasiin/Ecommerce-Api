using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetUSerOrders(string userId);
        Task<OrderDetailsResponse?> GetUSerOrder(string userId, int orderId);
        Task<bool> CancelOrder(string userId, int orderId);

        Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum status);
        Task <bool> ChangeOrderStatus (int  orderId, ChangeOrderStatusRequest request);

    }
}
