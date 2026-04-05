using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService , IStringLocalizer<SharedResources> localizer)
        {
            _brandService = brandService;
            _localizer= localizer;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var brands =await _brandService.GetAllBrandsAsync();
            return Ok(new { data = brands });


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var brand = await _brandService.GetBrand(b => b.Id==id);
                if (brand == null)
                return BadRequest();
            return Ok(new { data = brand });


        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)

        {
            var deleted =await _brandService.DeleteBrand(id);
            if (!deleted) return BadRequest();
            return Ok();

        }

        [HttpPost("")]
        [Authorize]

        public async Task<IActionResult> Create([FromForm]BrandRequest request)
        {
            await _brandService.CreateBrand(request);
            return Ok();
        }


    }
}
