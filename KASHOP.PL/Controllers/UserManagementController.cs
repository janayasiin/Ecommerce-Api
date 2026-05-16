using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize]
    public class UserManagementController : ControllerBase
    {

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService, IStringLocalizer<SharedResources> localizer)
        {

            _localizer = localizer;
            _userManagementService = userManagementService;
        }

        [HttpGet("users")]

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsers();
            return Ok(new { data = users });
        }

        [HttpGet("users/{userId}")]

        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var users = await _userManagementService.GetUser(userId);
            return Ok(new { data = users });
        }
        [HttpPatch("{userId}/role")]

        public async Task<IActionResult> ChangeRole([FromRoute] string userId , [FromBody] ChangeRoleRequest request)
        {
            var result = await _userManagementService.ChangeRole(userId, request.newRole);

            if (!result) return BadRequest();
        
        return Ok();
        
        }
        [HttpPatch("{userId}/toggle-block")]

        public async Task<IActionResult> ChangeRole([FromRoute] string userId)
        {
            var result = await _userManagementService.ToggleBlockUser(userId);

            if (!result) return BadRequest();

            return Ok();

        }



    }
}
