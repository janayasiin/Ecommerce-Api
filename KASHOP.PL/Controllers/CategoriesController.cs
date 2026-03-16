using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _categoryService = categoryService;
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _categoryService.CreateCategory(request);

            return Ok(new
            {
                message = _localizer["Success"].Value,
                response
            });

        }


        [HttpGet("")] 
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategorries();
            return Ok(new

            {
                data = categories,


                _localizer["Success"].Value
            });

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {



           return Ok(await _categoryService.GetCategory(c => c.Id == id));


        }
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _categoryService.DeleteCategory(id);
            if (!delete)
                return NotFound(new { message = _localizer["NotFound"].Value });

            return Ok(new { message = _localizer["Success"].Value });

        }

        public async Task<IActionResult> Update(int id, CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategory(id, request);
            if (result is null)
                return NotFound();

            return Ok(result);
        }


    }
}

