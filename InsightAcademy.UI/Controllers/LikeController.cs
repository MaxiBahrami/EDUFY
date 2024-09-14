using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.InfrastructureLayer.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightAcademy.UI.Controllers
{
    [Authorize]
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;
        private readonly ITutorService _tutorService;

        public LikeController(ILikeService likeService, ITutorService tutorService)
        {
            _likeService = likeService;
            _tutorService = tutorService;
        }

        //[ActionName("MyWishList")]
        public async Task<IActionResult> LikeList()
        {
            var likes = await _likeService.GetAllLike();
            return View("LikeList", likes);
        }

        [HttpPost]
        public async Task<IActionResult> AddLike(Guid tutorId)
        {
            if (tutorId != Guid.Empty)
            {
                var status = await _likeService.AddLike(tutorId);
                if (status)
                {
                    return Json(new { status = true, message = "" });
                }
                else
                {
                    return Json(new { status = false, message = "Server Error!" });
                }
            }
            return Json(new { status = false, message = "Server Error!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLike(Guid wishListId)
        {
            if (wishListId != Guid.Empty)
            {
                var status = await _likeService.DeleteLike(wishListId);
                if (status)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacherToLike(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                
                    var result = await _likeService.AddTeacherToLike(userName);
                    if (result != "")
                    {
                        TempData["success"] = "Tutor added in your like list";
                    }
                    else
                    {
                        TempData["error"] = "Tutor does not exist";
                    }
                
            }
            return Redirect("/Like/LikeList");
            
        }
    }
}