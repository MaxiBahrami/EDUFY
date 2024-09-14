using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsightAcademy.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		public async Task<IActionResult> CategoryList()
		{
			var categoryList = await _categoryService.GetAllCategories();
			return View(categoryList);
		}

		public IActionResult CreateCategory()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateCategory(Category category)
		{
			if (ModelState.IsValid)
			{
				await _categoryService.AddCategory(category);
				TempData["success"] = "Category Created Successfully";
				return Redirect("/Admin/Category/CategoryList");
			}
			ModelState.AddModelError("", "An error occurs while saving the category");
			return View(category);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var categoryFromDb = await _categoryService.GetCategoryById(id);
			return View(categoryFromDb);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Category category)
		{
			var categoryFromDb = await _categoryService.EditCategory(category);
			if (categoryFromDb == true)
			{
				TempData["success"] = "Category Edit Successfully";
				return Redirect("/Admin/Category/CategoryList");
			}
			ModelState.AddModelError("", "An error occurs while saving the category");
			return View(category);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var categoryFromDb = await _categoryService.GetCategoryById(id);
			return View(categoryFromDb);
		}

		[HttpPost]
		[ActionName("Delete")]
		public async Task<IActionResult> DeletePOST(Guid Id)
		{
			var categoryFromDb = await _categoryService.DeleteCategory(Id);
			if (categoryFromDb == true)
			{
				TempData["success"] = "Category Deleted Successfully";
				return Redirect("/Admin/Category/CategoryList");
			}
			ModelState.AddModelError("", "An error occurs while saving the category");
			return View(Id);
		}
	}
}