using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CategoryService : ICategoryService
        
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) { 
            _categoryRepository = categoryRepository;
        }
        public async Task <CategoryResponse> CreateCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
         await  _categoryRepository.CreateAsync(category);
            return category.Adapt<CategoryResponse>();

        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetOne(c => c.Id == id);
            if (category == null) { return false; }
                
            
            return await _categoryRepository.DeleteAsync(category);
        }

        public async Task<List<CategoryResponse>> GetAllCategorries()
        {
            var categories = await _categoryRepository.GetAllAsync(c=>c.Status==EntityStatus.Active,new string[] { nameof(Category.Translations), 
                nameof(Category.CreatedBy) });
            return categories.Adapt<List<CategoryResponse>>();
        }
        public async Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepository.GetOne(filter, new string[] { nameof(Category.Translations) });
            return  category.Adapt<CategoryResponse>();
        }
      

        public async Task<CategoryResponse?> UpdateCategory(int id, CategoryRequest request)
        {
            var category = await _categoryRepository.GetOne(
                c => c.Id == id,
                new string[] { nameof(Category.Translations) }
            );

            if (category is null)
                return null;

            foreach (var t in category.Translations)
            {
                var req = request.Translations
                    .FirstOrDefault(x => x.Language == t.Language);

                if (req is not null)
                    t.Name = req.Name;
            }

            await _categoryRepository.UpdateAsync(category);

            return category.Adapt<CategoryResponse>();


        }
    }
}
