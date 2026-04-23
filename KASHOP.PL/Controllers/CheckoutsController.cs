using KASHOP.BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutsController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutsController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("")]

        public async Task<IActionResult> Payment([FromBody] CheckoutRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _checkoutService.ProcessCheckout(UserId, request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<IActionResult> success([FromQuery] string sessionId)
        {
            var result = await _checkoutService.HandelSuccess(sessionId);
            return Ok(new { message = "Success" , sessionId = sessionId }); 
        }
    }
}
