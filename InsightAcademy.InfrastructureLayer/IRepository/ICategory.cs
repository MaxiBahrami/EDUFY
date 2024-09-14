using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
	public interface ICategory
	{
		Task<IEnumerable<Category>> GetAllCategories();

		Task<List<string>> GetAllTags();

		Task<bool> AddCategory(Category category);

		Task<bool> EditCategory(Category category);

		Task<bool> DeleteCategory(Guid categoryId);

		Task<Category> GetCategoryById(Guid Id);
	}
}