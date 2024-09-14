using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.DomainLayer.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightAcademy.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class DashboardController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly IStudentService _studentService;
        private readonly ICategoryService _categoryService;
        private readonly IDashBoardService _dashBoardService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;

        public DashboardController(ITutorService tutorService, IStudentService studentService, IDashBoardService dashBoardService, ICategoryService categoryService, ISubjectService subjectService, IUserService userService)
        {
            _tutorService = tutorService;
            _studentService = studentService;
            _dashBoardService = dashBoardService;
            _categoryService = categoryService;
            _subjectService = subjectService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            DashBoardViewModel dashBoardViewModel = new DashBoardViewModel()
            {
                Tutor = await _tutorService.GetTutorsByStatus(0),
                DashBoard = await _dashBoardService.GetDashBoardData(),
                NewSubjectRequests = await _subjectService.GetAllNewSubjectRequest(),
                RecentTransactions = await _dashBoardService.RecentTransactions(),
            };

            return View(dashBoardViewModel);
        }

        public async Task<IActionResult> ApprovedTutorList()
        {
            var tutor = await _tutorService.GetTutorsByStatus(1);
            return View(tutor);
        }

        public async Task<IActionResult> GetStudentList()
        {
            var studentList = await _studentService.GetStudentList();
            return View(studentList);
        }

        public async Task<IActionResult> GetLatestStudent()
        {
            var studentList = await _studentService.GetLatestStudent();
            return View(studentList);
        }

        public async Task<IActionResult> GetDashBoardData()
        {
            var studentList = await _dashBoardService.GetDashBoardData();
            return View(studentList);
        }

        //[HttpPost]
        //public async Task<IActionResult> LockUnlock(string tutorId)
        //{
        //    var result = await _dashBoardService.LockUnlock(tutorId);
        //    if (result == null)
        //    {
        //        return Json(new { success = true });
        //    }

        //    return Json(new { success = false });
        //}
        [HttpPost]
        public async Task<IActionResult> BlockTutor(string tutorId)
        {
            var result = await _dashBoardService.BlockTutor(tutorId);
            if (result == null)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UnblockTutor(string tutorId)
        {
            var result = await _dashBoardService.UnblockTutor(tutorId);
            if (result == null)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Guid tutorId)
        {
            var result = await _tutorService.ApproveTutor(tutorId);
            if (result == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Unapprove(Guid tutorId)
        {
            var result = await _tutorService.UnapproveTutor(tutorId);
            if (result == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(Guid tutorId)
        {
            var result = await _tutorService.UnapproveTutor(tutorId);
            if (result == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public async Task<IActionResult> GetAllBlockTutorList()
        {
            ViewBag.Title = "Block";
            var blockTutors = await _dashBoardService.GetAllBlockTutorList();
            return View(blockTutors);
        }

        [HttpPost]
        public async Task<IActionResult> BlockStudent(string studentId)
        {
            var result = await _userService.BlockStudent(studentId);
            if (result == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UnblockStudent(string studentId)
        {
            var result = await _userService.UnblockStudent(studentId);
            if (result == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public async Task<IActionResult> EditTutor(Guid Id)
        {
            var tutor = await _tutorService.tutorProfile(Id);
            return View(tutor);
        }

        [HttpPost]
        public async Task<IActionResult> EditTutor(Tutor tutorObj)
        {
            var tutor = await _tutorService.EditTutorByAdmin(tutorObj);
            if (tutor == true)
            {
                TempData["success"] = "Tutor Edit Successfully";
                return Redirect("/Admin/Dashboard/Index");
            }
            ModelState.AddModelError("", "An error occurs while saving the category");
            return View(tutor);
        }

        public async Task<IActionResult> EditStudentByAdmin(Guid Id)
        {
            var student = await _studentService.GetStudentById(Id);
            return View(student);
        }

        [HttpPost]
        [ActionName("EditStudentByAdmin")]
        public async Task<IActionResult> EditStudentByAdminPOST(ApplicationUser studentObj)
        {
            var student = await _studentService.EditStudentByAdmin(studentObj);
            if (student == true)
            {
                TempData["success"] = "Student Edit Successfully";
                return Redirect("/Admin/Dashboard/Index");
            }
            ModelState.AddModelError("", "An error occurs while saving the category");
            return View(student);
        }
    }
}