using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InsightAcademy.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly ICategoryService _categoryService;

        public SubjectController(ISubjectService subjectService, ICategoryService categoryService)
        {
            _subjectService = subjectService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> SubjectList()
        {
            var categoryList = await _subjectService.GetAllSubject();
            return View(categoryList);
        }

        public async Task<IActionResult> CreateSubject()
        {
            ViewBag.CategoryList = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            return View();
        }

        public async Task<IActionResult> GetAllSubjectRequest()
        {
            var subjectsRequest = await _subjectService.GetAllNewSubjectRequest();
            return View(subjectsRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectService.AddSubject(subject);
                TempData["success"] = "Subject Created Successfully";
                return Redirect("/Admin/Subject/SubjectList");
            }
            ViewBag.CategoryList = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            ModelState.AddModelError("", "An error occurs while saving the category");
            return View(subject);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.CategoryList = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            var subjectFromDb = await _subjectService.GetSubjectById(id);
            return View(subjectFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            var subjectFromDb = await _subjectService.EditSubject(subject);
            if (subjectFromDb == true)
            {
                TempData["success"] = "Subject Edit Successfully";
                return Redirect("/Admin/Subject/SubjectList");
            }
            ViewBag.CategoryList = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            ModelState.AddModelError("", "An error occurs while saving the category");
            return View(subject);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var categoryFromDb = await _subjectService.GetSubjectById(id);
            return View(categoryFromDb);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(Guid Id)
        {
            var categoryFromDb = await _subjectService.DeleteSubjectById(Id);
            if (categoryFromDb == true)
            {
                TempData["success"] = "Category Deleted Successfully";
                return Redirect("/Admin/Subject/SubjectList");
            }
            ModelState.AddModelError("", "An error occurs while saving the category");
            return View(Id);
        }
    }
}