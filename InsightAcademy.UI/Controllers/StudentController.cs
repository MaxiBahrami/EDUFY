using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using InsightAcademy.UI.Extensions;
using InsightAcademy.UI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InsightAcademy.UI.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IWishList _wishListService;
        private readonly ITutorService _tutorService;
        private readonly IUserService _userService;
        private readonly FileUploader _fileUploader;
        private readonly IReviewService _reviewService;
        private readonly IGeoService _geoService;

        public StudentController(IWishList wishListService,
            ITutorService tutorService,
            IUserService userService,
            FileUploader fileUploader,
            IReviewService reviewService,
            IGeoService geoService

            )
        {
            _wishListService = wishListService;
            _tutorService = tutorService;
            _userService = userService;
            _fileUploader = fileUploader;
            _reviewService = reviewService;
            _geoService = geoService;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = User.GetUserId();
            var user = await _userService.GetUserDetailById(userId);
            ViewBag.Country = new SelectList(await _geoService.GetAllCountries(), "Id", "Name");
            if (int.TryParse(user.Country, out int countryId))
            {
                ViewBag.Cities = new SelectList(await _geoService.GetCitiesByCountryId(countryId), "Id", "Name");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ApplicationUser user)
        {
            var userId = User.GetUserId();
            var result = await _fileUploader.UploadFile(user.UserImage);
            if (result.status)
            {
                user.ProfileImageUrl = result.url;
            }

            await _userService.UpdateProfile(userId, user);
            return Redirect("/Student/Profile");
        }

        public async Task<IActionResult> AddReview(string feedBack, int rating, Guid tutorId)
        {
            var status = await _reviewService.AddReview(feedBack, rating, tutorId);
            if (status)
            {
                return Json(new { status = true, message = "" });
            }
            else
            {
                return Json(new { status = false, message = "Server Error!" });
            }
        }

        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.GetUserId();
            var result = await _userService.GetMyBookings(userId);

            ViewBag.FutureBookings = result.futureBookings;
            ViewBag.PastBookings = result.pastBookings;

            return View();
        }

        //public async Task<IActionResult> AllReviews()
        //{
        //	var ReviewList = await _reviewService.GetAllReviews();
        //	return PartialView("_Review", ReviewList);
        //}
    }
}