using InsightAcademy.ApplicationLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsightAcademy.UI.Controllers
{
    public class ReviewsController : Controller
    {
        private IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> FilterReviews(double rating, Guid tutorId, string order = "asc")
        {
            var reviews = await _reviewService.FilterReviews(rating, order, tutorId);
            return PartialView("_Review", reviews);
        }
    }
}