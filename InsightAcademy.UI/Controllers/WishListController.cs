using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsightAcademy.UI.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IWishListService _wishListService;
        private readonly ITutorService _tutorService;

        public WishListController(IWishListService wishListService, ITutorService tutorService)
        {
            _wishListService = wishListService;
            _tutorService = tutorService;
        }

        [ActionName("MyWishList")]
        public async Task<IActionResult> GetAllWishList()
        {
            var wishList = await _wishListService.GetAllWishList();
            return View("GetAllWishList", wishList);
        }

        [HttpPost]
        public async Task<IActionResult> AddWishList(Guid tutorId)
        {
            if (tutorId != Guid.Empty)
            {
                if (await _wishListService.IsAlreadyExist(User.FindFirstValue(ClaimTypes.NameIdentifier), tutorId))
                {
                    return Json(new { status = false, message = "This tutor already added in your wishlist" });
                }
                var status = await _wishListService.AddWishList(tutorId);
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
        public async Task<IActionResult> DeleteWishList(Guid wishListId)
        {
            if (wishListId != Guid.Empty)
            {
                var status = await _wishListService.DeleteWishList(wishListId);
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
    }
}