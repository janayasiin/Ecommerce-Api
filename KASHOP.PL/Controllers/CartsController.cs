using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICartService _cartService;

        public CartsController(IStringLocalizer<SharedResources> localizer, ICartService cartService)
        {
            _localizer = localizer;
            _cartService = cartService;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(AddtoCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

             await _cartService.AddToCart(request,UserId);

            return Ok(new
            {
                message = _localizer["Success"].Value,
              
            });

        }
    }
}
