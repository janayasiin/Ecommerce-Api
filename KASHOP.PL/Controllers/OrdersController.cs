using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Stripe.Climate;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService, IStringLocalizer<SharedResources> localizer)
        {
            
            _orderService = orderService;
            _localizer = localizer;

            
        }
        [HttpGet("")]
        public async Task <IActionResult> GetMyOrders (){
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders =await _orderService.GetUSerOrders(userId);
            return Ok( new { data = orders });
        
        
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetUSerOrder(userId,orderId);
            return Ok(new { data = orders });


        }
        [HttpGet("admin")]
        public async Task<IActionResult> CancelOrder([FromQuery]OrderStatusEnum status = OrderStatusEnum.Pending)
        {
            var orders = await _orderService.GetAllOrders(status);
            return Ok(new { data = orders });

        }


        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id , [FromBody] ChangeOrderStatusRequest status )
        {
            var result = await _orderService.ChangeOrderStatus(id,status);
            if(!result) return BadRequest();
            return Ok();

            

        }
    }
}
