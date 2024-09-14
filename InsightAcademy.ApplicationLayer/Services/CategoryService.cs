using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
	public class CategoryService : ICategoryService
	{
		private ICategory _category;

		public CategoryService(ICategory category)
		{
			_category = category;
		}

		public async Task<IEnumerable<Category>> GetAllCategories()
		{
			return await _category.GetAllCategories();
		}

		public async Task<List<string>> GetAllTags()
		{
			return await _category.GetAllTags();
		}

		public async Task<bool> AddCategory(Category category)
		{
			return await _category.AddCategory(category);
		}

		public async Task<bool> EditCategory(Category category)
		{
			return await _category.EditCategory(category);
		}

		public async Task<bool> DeleteCategory(Guid categoryId)
		{
			return await _category.DeleteCategory(categoryId);
		}

		public async Task<Category> GetCategoryById(Guid Id)
		{
			return await _category.GetCategoryById(Id);
		}
	}
}