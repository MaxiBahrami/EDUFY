using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.IRepository;
using InsightAcademy.UI.Hubs;
using InsightAcademy.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Stripe.Checkout;
using System.Diagnostics;

namespace InsightAcademy.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICategoryService _categoryService;
        private readonly IEmailService _emaiService;
        private readonly ILogger<HomeController> _logger;
        private readonly IAvailabilityRepository _availability;

        public HomeController(ILogger<HomeController> logger, ITutorService tutorService, ICategoryService categoryService, IAvailabilityRepository availabilityRepository, IEmailService emailService)
        {
            _tutorService = tutorService;
            _logger = logger;
            _categoryService = categoryService;
            _availability = availabilityRepository;
            _emaiService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Subject = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            return View();
        }

        public IActionResult SearchListing()
        {
            return View();
        }

        public IActionResult OrderSummary()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> OrderConfirmation(Guid bookingId)
        {
            var sessionId = HttpContext.Session.GetString("SessionId");

            if (!string.IsNullOrEmpty(sessionId))
            {
                var service = new SessionService();
                Session session = service.Get(sessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    var booking = await _availability.UpdateBookingStatus(bookingId);

                    var payment = new Payment()
                    {
                        BookingId = bookingId,
                        PaymentMethod = "Card",
                        Amount = Convert.ToDecimal(session.AmountTotal),
                        CreatedDateTime = DateTime.Now,
                        PaymentDate = DateTime.Now,
                    };

                    await _availability.AddPayment(payment);

                    await _emaiService.SendStudentBookingConfirmationEmail(booking.TutorId, booking.StudentId.ToString(), booking.BookingDate.ToShortDateString(), booking.StartTime, booking.EndTime);
                    await _emaiService.SendTutorBookingConfirmationEmail(booking.TutorId, booking.StudentId.ToString(), booking.BookingDate.ToShortDateString(), booking.StartTime, booking.EndTime);
                }
            }
            return Redirect($"/Home/OrderSummary");
        }

        public async Task<IActionResult> CancelTransaction()
        {
            return View();
        }

        #region Paypal

        public async Task<IActionResult> PaymentSuccess(string paymentId, Guid bookingId, decimal amount)
        {
            var booking = await _availability.UpdateBookingStatus(bookingId);

            var payment = new Payment()
            {
                BookingId = bookingId,
                PaymentMethod = "Card",
                Amount = amount,
                CreatedDateTime = DateTime.Now,
                PaymentDate = DateTime.Now,
            };

            await _availability.AddPayment(payment);

            await _emaiService.SendStudentBookingConfirmationEmail(booking.TutorId, booking.StudentId.ToString(), booking.BookingDate.ToShortDateString(), booking.StartTime, booking.EndTime);
            await _emaiService.SendTutorBookingConfirmationEmail(booking.TutorId, booking.StudentId.ToString(), booking.BookingDate.ToShortDateString(), booking.StartTime, booking.EndTime);
            return View("OrderSummary");
        }

        public IActionResult PaymentCancel()
        {
            // Handle payment cancellation
            return View();
        }

        public IActionResult PaymentFailed()
        {
            // Handle payment failure
            return View();
        }

        #endregion Paypal
    }
}