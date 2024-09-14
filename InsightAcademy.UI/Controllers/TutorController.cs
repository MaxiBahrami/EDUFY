using AspNetCoreHero.ToastNotification.Abstractions;
using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Models;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using InsightAcademy.UI.Extensions;
using InsightAcademy.UI.Helper;
using InsightAcademy.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NodaTime;
using Stripe;
using Stripe.Checkout;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace InsightAcademy.UI.Controllers
{
    [Authorize(Roles = "Teacher,SuperAdmin")]
    public class TutorController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly IWishList _wishListService;
        private readonly IUser _user;
        private readonly FileUploader _fileUploader;
        private readonly ICategoryService _categoryService;
        private readonly ISubjectService _subjectService;
        private readonly INotyfService _notyf;
        private readonly IGeoService _geoService;
        private readonly IAvailabilityService _availabilityService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        //public Pager<TutorSubject> Pager;
        public Pager<Tutor> Pager;

        public class VideoData
        {
            public string Url { get; set; }
        }

        public TutorController(ITutorService tutorService,
            IUser user,
            FileUploader fileUploader,
            IWishList wishListService,
            ICategoryService categoryService,
            INotyfService notyf,
            ISubjectService subjectService,
            IGeoService geoService,
            HttpClient httpClient,
            IAvailabilityService availabilityService,
            IConfiguration config
            )
        {
            _tutorService = tutorService;
            _user = user;
            _fileUploader = fileUploader;
            _wishListService = wishListService;
            _categoryService = categoryService;
            _notyf = notyf;
            _subjectService = subjectService;
            _geoService = geoService;
            _availabilityService = availabilityService;
            _httpClient = httpClient;
            _config = config;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/check-auth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new { isAuthenticated = true });
            }
            return Ok(new { isAuthenticated = false });
        }

        [HttpGet]
        public IActionResult UpcomingBookings()
        {
            return View();
        }

        public async Task<IActionResult> GetUpcomingBookingsData()
        {
            List<dynamic> calanderData = new List<dynamic>();
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);
            var tutor = await _tutorService.GetTutor(tutorId);
            var tutorUser = await _user.GetUserDetailById(userId);
            var myBookings = await _availabilityService.GetMyBookings(tutorId.ToString());

            foreach (var order in myBookings)
            {
                var student = await _user.GetUserDetailById(order.StudentId.ToString());
                var sb = new StringBuilder();

                // Get the time zones for the tutor and the student
                var tutorTimeZone = DateTimeZoneProviders.Tzdb[tutorUser.TimeZone];
                var studentTimeZone = DateTimeZoneProviders.Tzdb[student.TimeZone];

                // Create the student's local time using the provided TimeSpan (assumed to be in student's time zone)
                var studentStartTime = new LocalDateTime(2024, 9, 5, order.StartTime.Hours, order.StartTime.Minutes);
                var studentEndTime = new LocalDateTime(2024, 9, 5, order.EndTime.Hours, order.EndTime.Minutes);

                // Convert student's local time to a ZonedDateTime using the student's time zone
                var studentStartTimeZoned = studentTimeZone.AtLeniently(studentStartTime);
                var studentEndTimeZoned = studentTimeZone.AtLeniently(studentEndTime);

                // Convert the student's local time (ZonedDateTime) to UTC
                var studentStartTimeUTC = studentStartTimeZoned.ToInstant();
                var studentEndTimeUTC = studentEndTimeZoned.ToInstant();

                // Convert UTC time to the tutor's local time zone
                var tutorStartTimeLocal = studentStartTimeUTC.InZone(tutorTimeZone);
                var tutorEndTimeLocal = studentEndTimeUTC.InZone(tutorTimeZone);

                // Convert the tutor's local time back to TimeSpan for display or further calculations
                string tutorFormattedStartTime = tutorStartTimeLocal.ToString("hh:mm tt", null); // "tt" adds AM/PM
                string tutorFormattedEndTime = tutorEndTimeLocal.ToString("hh:mm tt", null); // "tt" adds AM/PM

                sb.AppendLine("<strong>Booking Information</strong> <br/>");
                sb.AppendLine($"Student Name: {student.FullName} <br>");
                sb.AppendLine($"Booking Date: {order.BookingDate.ToString("yyyy-MM-dd")}<br>");
                sb.AppendLine($"StartTime: {tutorFormattedStartTime}<br>");
                sb.AppendLine($"EndTime: {tutorFormattedEndTime}<br>");

                sb.AppendLine($"<a  href='{tutor.GoogleMeetLink}' target='_blank'>Join</a><br>");

                calanderData.Add(new
                {
                    id = order.Id,
                    title = $"Metting with {student.FullName} ",
                    description = sb.ToString(),

                    start = order.BookingDate.ToString("yyyy-MM-dd")
                });
            }
            return new JsonResult(calanderData);
        }

        public async Task<IActionResult> PersonalDetails()
        {
            var userId = User.GetUserId();
            var tutor = await _tutorService.GetPersonalDetail(userId);
            ViewBag.Country = new SelectList(await _geoService.GetAllCountries(), "Id", "Name");

            if (int.TryParse(tutor.Country, out int countryId))
            {
                ViewBag.Cities = new SelectList(await _geoService.GetCitiesByCountryId(countryId), "Id", "Name");
            }

            if (userId == null)
            {
                return View(new Tutor());
            }

            if (tutor == null)
            {
                return View(new Tutor());
            }

            return View(tutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalDetails(Tutor profile)
        {
            if (ModelState.IsValid)
            {
                if (profile.Id == Guid.Empty)
                {
                    profile.ApplicationUserId = User.GetUserId();
                    await _tutorService.AddPersonalDetialAsync(profile);
                    _notyf.Success("Personal Details has been added");
                }
                else
                {
                    var status = await _tutorService.EditPersonalDetails(profile);
                    if (status)
                    {
                        _notyf.Success("Personal details has been updated");
                    }
                }

                return RedirectToAction("ContactDetail", "Tutor");
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(profile);
        }

        public async Task<IActionResult> ContactDetail()
        {
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);

            if (tutorId == Guid.Empty)
            {
                var tutorDetials = await _user.GetUserDetailById(userId);
                var contactObj = new Contact();
                contactObj.Email = tutorDetials.Email;
                contactObj.PhoneNumber = tutorDetials.PhoneNumber;
                contactObj.Website = tutorDetials.Website;
                return View(contactObj);
            }

            var contact = await _tutorService.GetContactDetails(tutorId);

            if (contact == null)
            {
                return View(new Contact());
            }

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactDetail(Contact contact)
        {
            if (ModelState.IsValid)
            {
                if (contact.Id == Guid.Empty)
                {
                    var userId = User.GetUserId();
                    var tutorId = await _user.GetTutorId(userId);
                    contact.TutorId = tutorId;
                    await _tutorService.AddContactDetialAsync(contact);
                    _notyf.Success("Contact details has been added");
                }
                else
                {
                    var status = await _tutorService.EditContactDetails(contact);
                    if (status)
                    {
                        _notyf.Success("Contact details has been updated");
                    }
                }

                return RedirectToAction("Education", "Tutor");
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(contact);
        }

        public async Task<IActionResult> EducationById(Guid id)
        {
            var education = await _tutorService.GetEducationById(id);
            return Json(education);
        }

        public async Task<IActionResult> Education()
        {
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);

            if (tutorId == Guid.Empty)
            {
                return View(new Education());
            }

            var education = await _tutorService.GetEducationDetails(tutorId);
            if (education == null)
            {
                return View(new Education());
            }
            ViewBag.Education = education;

            return View(new Education());
        }

        [HttpPost]
        public async Task<IActionResult> Education(Education education)
        {
            bool isOK = false;
            if (ModelState.IsValid)
            {
                if (education.Id == Guid.Empty)
                {
                    var userId = User.GetUserId();
                    var tutorId = await _user.GetTutorId(userId);
                    education.TutorId = tutorId;

                    await _tutorService.AddEducationDetialAsync(education);
                    _notyf.Success("Education has been added");
                }
                else
                {
                    var status = await _tutorService.EditEducationDetails(education);
                    if (status)
                    {
                        _notyf.Success("Education has been updated");
                    }
                }

                return RedirectToAction("Education", "Tutor");
            }
            else
            {
                isOK = true;
                ViewBag.HasErrors = isOK;
                return View(education);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEducation(Guid id)
        {
            await _tutorService.DeleteEducation(id);
            return Json(true);
        }

        public async Task<IActionResult> Media()
        {
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);
            var tutor = await _tutorService.GetTutorMedia(tutorId);
            return View(tutor);
        }

        [Route("[Controller]/Profile/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ProfileView(Guid id)
        {
            if (id != Guid.Empty)
            {
                ViewBag.ZonesList = LoadTimeZones();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await GetTimeZone(ipAddress);
                var tutor = await _tutorService.tutorProfile(id);
                if (tutor != null)
                {
                    ViewBag.RelatedTeachers = await _tutorService.GetRelatedTutorsBySubject(tutor);
                    ViewBag.HasReviewed = tutor.Reviews.Any(r => r.StudentId == User.GetUserId());
                    return View(tutor);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> NewSubjectRequest()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            return View(new NewSubjectRequest());
        }

        [HttpPost]
        public async Task<IActionResult> NewSubjectRequest(NewSubjectRequest newSubjectRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var status = await _tutorService.AddNewSubjectRequest(newSubjectRequest);
            return View(new NewSubjectRequest());
        }

        public async Task<IActionResult> Subjects()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            ViewBag.AllSubjects = new SelectList(await _subjectService.GetAllSubject(), "Id", "Name");
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);

            if (tutorId == Guid.Empty)
            {
                return View(new TutorSubject());
            }

            var subjects = await _tutorService.GetMySubjects(tutorId);
            ViewBag.Subjects = subjects;
            //if (education == null)
            //{
            //    return View(new Education());
            //}

            return View(new TutorSubject());
        }

        [HttpPost]
        public async Task<IActionResult> Subjects(TutorSubject subject)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId();
                if (subject.Id == Guid.Empty)
                {
                    subject.TutorId = await _user.GetTutorId(userId);
                    //var status = await _tutorService.AddSubjectAsync(subject);

                    await _tutorService.AddTutorSubjectAsync(subject);
                    _notyf.Success("Subject has been added");
                }
                else
                {
                    subject.TutorId = await _user.GetTutorId(userId);
                    var status = await _tutorService.EditTutorSubjectDetails(subject);
                    if (status)
                    {
                        _notyf.Success("Subject has been added");
                    }
                }
                return RedirectToAction(nameof(Subjects));
            }
            return View(subject);
        }

        public async Task<IActionResult> GetSubjectById(Guid id)
        {
            var subject = await _tutorService.GetSubjectById(id);
            return Json(subject);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            await _tutorService.DeleteSubject(id);
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(IFormFile profilePic)
        {
            if (profilePic != null)
            {
                var result = await _fileUploader.UploadFile(profilePic);
                if (result.status)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var status = await _user.UploadProfileImage(userId, result.url);
                    if (status)
                    {
                        return Json(new { status = status, Url = result.url });
                    }
                }
                else
                {
                    return Json(new { status = false, Url = "" });
                }
            }
            return Json(new { status = false, Url = "" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Filter(TutorFilter filter, int pageNumber = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                filter.CurrentUserCountryId = _user.GetUserDetailById(User.GetUserId()).Result.City;
            }
            var tutors = await _tutorService.Filter(filter, pageNumber);
            if (filter.FilterType == 1)
            {
                return Json(new { partialView = await this.RenderViewAsync("_FilterTutor", tutors, true), totalCount = tutors.TotalItems });
            }
            else
            {
                return Json(new { partialView = await this.RenderViewAsync("_TutorList2", tutors, true), totalCount = tutors.TotalItems });
            }
        }

        [ActionName("list")]
        [AllowAnonymous]
        public async Task<IActionResult> AllTutor(TutorFilter filter, int pageNumber = 1)
        {
            ViewBag.Degree = new SelectList(await _tutorService.GetAllDegree(), "Degree", "Degree");
            ViewBag.Subject = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            var IsAuthenticated = User.Identity.IsAuthenticated;

            ViewBag.HeaderTags = await _categoryService.GetAllTags();
            ViewBag.SearchSubject = filter.Tags;

            var tutorList = await _tutorService.Filter(filter, pageNumber);

            return View("AllTutor", tutorList);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetTutorRatingBreakDown(Guid tutorId)
        {
            var ratingBreakdown = await _tutorService.GetTutorRatingBreakDown(tutorId);

            return new JsonResult(ratingBreakdown);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GridView(TutorFilter filter, int pageNumber = 1)
        {
            ViewBag.Degree = new SelectList(await _tutorService.GetAllDegree(), "Degree", "Degree");
            ViewBag.Subject = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            ViewBag.HeaderTags = await _categoryService.GetAllTags();
            ViewBag.SearchSubject = filter.Tags;
            var tutorList = await _tutorService.Filter(filter, pageNumber);

            return View(tutorList);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMedia(List<IFormFile> mediaFiles)
        {
            // Define image and video extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var userId = User.GetUserId();
            var tutorId = await _user.GetTutorId(userId);

            if (mediaFiles.Count > 0)
            {
                foreach (var file in mediaFiles)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (imageExtensions.Contains(fileExtension))
                    {
                        var fileUploadresult = await _fileUploader.UploadFile(file);
                        if (fileUploadresult.status)
                        {
                            await _tutorService.UploadMedia(tutorId, fileUploadresult.url);
                        }
                    }
                    // Check if the file extension matches any of the video extensions
                    else
                    {
                        return Json(new { status = false, message = "Only JPEG and PNG images are allowed" });
                    }
                }
            }

            return Json(new { status = true, message = "" });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePicture(Guid id)
        {
            await _tutorService.DeleteMediaUrl(id);
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> SaveVideo([FromBody] VideoData data)
        {
            // Define image and video extensions
            var userId = User.GetUserId();

            var tutorId = await _user.GetTutorId(userId);
            var tutorList = await _tutorService.SaveMediaUrl(tutorId, data.Url);
            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Availability()
        {
            ViewBag.ZonesList = LoadTimeZones();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await GetTimeZone(ipAddress);
            var tutorId = await _user.GetTutorId(User.GetUserId());
            var schedule = await _availabilityService.GetMySchedule(tutorId.ToString());
            var tutors = await _tutorService.GetTutor(tutorId);
            ViewBag.GoogleMeetLink = tutors.GoogleMeetLink ?? "";

            if (schedule.Monday.Count == 0)
            {
                schedule = null;
            }
            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Availability(AvailabilityModel availabilityModel)
        {
            if (availabilityModel == null)
            {
                return BadRequest();
            }

            await _availabilityService.AddOrUpdateAvailability(availabilityModel);
            return new JsonResult(true);
        }

        private async Task GetTimeZone(string ipAddress)
        {
            if (User.Identity.IsAuthenticated)
            {
                var timeZone = await _user.GetUserTimeZone(User.GetUserId());
                if (timeZone == "")
                {
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        ViewBag.TimeZone = await GetTimeZoneByIpAsync(ipAddress);
                    }
                }
                else
                {
                    ViewBag.TimeZone = timeZone;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    ViewBag.TimeZone = await GetTimeZoneByIpAsync(ipAddress);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorSlots(Guid tutorId, string date, string studentIANATimeZone)
        {
            var availableSlots = await _availabilityService.GetTutorSlots(tutorId, date, studentIANATimeZone);
            var bookedSlots = await _availabilityService.GetBookedSlots(tutorId, Convert.ToDateTime(date));
            return new JsonResult(new { schedule = availableSlots, bookedSlots = bookedSlots });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> BookSlot(string studentTimeZone, Guid tutorId, string bookingDate, string slot)
        {
            var userId = User.GetUserId();
            var timeZone = await _user.GetUserTimeZone(userId);

            if (timeZone != studentTimeZone)
            {
                await _user.UpdateTimeZone(studentTimeZone, userId);
            }
            var bookingId = await _availabilityService.BookSlot(tutorId, userId, bookingDate, slot);
            var Url = await ChargePayment(bookingId, tutorId);
            return new JsonResult(Url);
        }

        private async Task<string> ChargePayment(string bookingID, Guid tutorId)
        {
            var tutor = await _tutorService.GetTutor(tutorId);
            var domain = _config["Stripe:Domain"];
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"home/OrderConfirmation?bookingId= {bookingID}",
                CancelUrl = domain + $"home/CancelTransaction",
                //CustomerEmail = customerEmail,
            };

            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)tutor.HourlyRate * 100,//20.00 -> 2000
                    Currency = "USD",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"One hour session with {tutor.FullName} ",
                    },
                },
                Quantity = 1,
            };
            options.LineItems.Add(sessionLineItem);

            var service = new SessionService();

            Session session = service.Create(options);

            HttpContext.Session.SetString("SessionId", session.Id);
            return session.Url;
        }

        private List<string> LoadTimeZones()
        {
            return new List<string>() {  "Africa/Abidjan", "Africa/Accra", "Africa/Addis_Ababa", "Africa/Algiers", "Africa/Asmara",
    "Africa/Asmera", "Africa/Bamako", "Africa/Bangui", "Africa/Banjul", "Africa/Bissau",
    "Africa/Blantyre", "Africa/Brazzaville", "Africa/Bujumbura", "Africa/Cairo", "Africa/Casablanca",
    "Africa/Ceuta", "Africa/Conakry", "Africa/Dakar", "Africa/Dar_es_Salaam", "Africa/Djibouti",
    "Africa/Douala", "Africa/El_Aaiun", "Africa/Freetown", "Africa/Gaborone", "Africa/Harare",
    "Africa/Johannesburg", "Africa/Juba", "Africa/Kampala", "Africa/Khartoum", "Africa/Kigali",
    "Africa/Kinshasa", "Africa/Lagos", "Africa/Libreville", "Africa/Lome", "Africa/Luanda",
    "Africa/Lubumbashi", "Africa/Lusaka", "Africa/Malabo", "Africa/Maputo", "Africa/Maseru",
    "Africa/Mbabane", "Africa/Mogadishu", "Africa/Monrovia", "Africa/Nairobi", "Africa/Ndjamena",
    "Africa/Niamey", "Africa/Nouakchott", "Africa/Ouagadougou", "Africa/Porto-Novo", "Africa/Sao_Tome",
    "Africa/Timbuktu", "Africa/Tripoli", "Africa/Tunis", "Africa/Windhoek", "America/Adak",
    "America/Anchorage", "America/Anguilla", "America/Antigua", "America/Araguaina",
    "America/Argentina/Buenos_Aires", "America/Argentina/Catamarca", "America/Argentina/ComodRivadavia",
    "America/Argentina/Cordoba", "America/Argentina/Jujuy", "America/Argentina/La_Rioja",
    "America/Argentina/Mendoza", "America/Argentina/Rio_Gallegos", "America/Argentina/Salta",
    "America/Argentina/San_Juan", "America/Argentina/San_Luis", "America/Argentina/Tucuman",
    "America/Argentina/Ushuaia", "America/Aruba", "America/Asuncion", "America/Atikokan",
    "America/Atka", "America/Bahia", "America/Bahia_Banderas", "America/Barbados", "America/Belem",
    "America/Belize", "America/Blanc-Sablon", "America/Boa_Vista", "America/Bogota", "America/Boise",
    "America/Buenos_Aires", "America/Cambridge_Bay", "America/Campo_Grande", "America/Cancun",
    "America/Caracas", "America/Catamarca", "America/Cayenne", "America/Cayman", "America/Chicago",
    "America/Chihuahua", "America/Ciudad_Juarez", "America/Coral_Harbour", "America/Cordoba",
    "America/Costa_Rica", "America/Creston", "America/Cuiaba", "America/Curacao", "America/Danmarkshavn",
    "America/Dawson", "America/Dawson_Creek", "America/Denver", "America/Detroit", "America/Dominica",
    "America/Edmonton", "America/Eirunepe", "America/El_Salvador", "America/Ensenada", "America/Fort_Nelson",
    "America/Fort_Wayne", "America/Fortaleza", "America/Glace_Bay", "America/Godthab", "America/Goose_Bay",
    "America/Grand_Turk", "America/Grenada", "America/Guadeloupe", "America/Guatemala", "America/Guayaquil",
    "America/Guyana", "America/Halifax", "America/Havana", "America/Hermosillo", "America/Indiana/Indianapolis",
    "America/Indiana/Knox", "America/Indiana/Marengo", "America/Indiana/Petersburg", "America/Indiana/Tell_City",
    "America/Indiana/Vevay", "America/Indiana/Vincennes", "America/Indiana/Winamac", "America/Indianapolis",
    "America/Inuvik", "America/Iqaluit", "America/Jamaica", "America/Jujuy", "America/Juneau",
    "America/Kentucky/Louisville", "America/Kentucky/Monticello", "America/Knox_IN", "America/Kralendijk",
    "America/La_Paz", "America/Lima", "America/Los_Angeles", "America/Louisville", "America/Lower_Princes",
    "America/Maceio", "America/Managua", "America/Manaus", "America/Marigot", "America/Martinique",
    "America/Matamoros", "America/Mazatlan", "America/Mendoza", "America/Menominee", "America/Merida",
    "America/Metlakatla", "America/Mexico_City", "America/Miquelon", "America/Moncton", "America/Monterrey",
    "America/Montevideo", "America/Montreal", "America/Montserrat", "America/Nassau", "America/New_York",
    "America/Nipigon", "America/Nome", "America/Noronha", "America/North_Dakota/Beulah",
    "America/North_Dakota/Center", "America/North_Dakota/New_Salem", "America/Nuuk", "America/Ojinaga",
    "America/Panama", "America/Pangnirtung", "America/Paramaribo", "America/Phoenix", "America/Port_of_Spain",
    "America/Port-au-Prince", "America/Porto_Acre", "America/Porto_Velho", "America/Puerto_Rico",
    "America/Punta_Arenas", "America/Rainy_River", "America/Rankin_Inlet", "America/Recife", "America/Regina",
    "America/Resolute", "America/Rio_Branco", "America/Rosario", "America/Santa_Isabel", "America/Santarem",
    "America/Santiago", "America/Santo_Domingo", "America/Sao_Paulo", "America/Scoresbysund", "America/Shiprock",
    "America/Sitka", "America/St_Barthelemy", "America/St_Johns", "America/St_Kitts", "America/St_Lucia",
    "America/St_Thomas", "America/St_Vincent", "America/Swift_Current", "America/Tegucigalpa", "America/Thule",
    "America/Thunder_Bay", "America/Tijuana", "America/Toronto", "America/Tortola", "America/Vancouver",
    "America/Virgin", "America/Whitehorse", "America/Winnipeg", "America/Yakutat", "America/Yellowknife",
    "Antarctica/Casey", "Antarctica/Davis", "Antarctica/DumontDUrville", "Antarctica/Macquarie", "Antarctica/Mawson",
    "Antarctica/McMurdo", "Antarctica/Palmer", "Antarctica/Rothera", "Antarctica/South_Pole", "Antarctica/Syowa",
    "Antarctica/Troll", "Antarctica/Vostok", "Arctic/Longyearbyen", "Asia/Aden", "Asia/Almaty", "Asia/Amman",
    "Asia/Anadyr", "Asia/Aqtau", "Asia/Aqtobe", "Asia/Ashgabat", "Asia/Ashkhabad", "Asia/Atyrau", "Asia/Baghdad",
    "Asia/Bahrain", "Asia/Baku", "Asia/Bangkok", "Asia/Barnaul", "Asia/Beirut", "Asia/Bishkek", "Asia/Brunei",
    "Asia/Calcutta", "Asia/Chita", "Asia/Choibalsan", "Asia/Chongqing", "Asia/Chungking", "Asia/Colombo",
    "Asia/Dacca", "Asia/Damascus", "Asia/Dhaka", "Asia/Dili", "Asia/Dubai", "Asia/Dushanbe", "Asia/Famagusta",
    "Asia/Gaza", "Asia/Harbin", "Asia/Hebron", "Asia/Ho_Chi_Minh", "Asia/Hong_Kong", "Asia/Hovd", "Asia/Irkutsk",
    "Asia/Istanbul", "Asia/Jakarta", "Asia/Jayapura", "Asia/Jerusalem", "Asia/Kabul", "Asia/Kamchatka", "Asia/Karachi",
    "Asia/Kashgar", "Asia/Kathmandu", "Asia/Katmandu", "Asia/Khandyga", "Asia/Kolkata", "Asia/Krasnoyarsk",
    "Asia/Kuala_Lumpur", "Asia/Kuching", "Asia/Kuwait", "Asia/Macao", "Asia/Macau", "Asia/Magadan", "Asia/Makassar",
    "Asia/Manila", "Asia/Muscat", "Asia/Nicosia", "Asia/Novokuznetsk", "Asia/Novosibirsk", "Asia/Omsk", "Asia/Oral",
    "Asia/Phnom_Penh", "Asia/Pontianak", "Asia/Pyongyang", "Asia/Qatar", "Asia/Qostanay", "Asia/Qyzylorda",
    "Asia/Rangoon", "Asia/Riyadh", "Asia/Saigon", "Asia/Sakhalin", "Asia/Samarkand", "Asia/Seoul", "Asia/Shanghai",
    "Asia/Singapore", "Asia/Srednekolymsk", "Asia/Taipei", "Asia/Tashkent", "Asia/Tbilisi", "Asia/Tehran", "Asia/Tel_Aviv",
    "Asia/Thimbu", "Asia/Thimphu", "Asia/Tokyo", "Asia/Tomsk", "Asia/Ujung_Pandang", "Asia/Ulaanbaatar", "Asia/Ulan_Bator",
    "Asia/Urumqi", "Asia/Ust-Nera", "Asia/Vientiane", "Asia/Vladivostok", "Asia/Yakutsk", "Asia/Yangon", "Asia/Yekaterinburg",
    "Asia/Yerevan", "Atlantic/Azores", "Atlantic/Bermuda", "Atlantic/Canary", "Atlantic/Cape_Verde", "Atlantic/Faeroe",
    "Atlantic/Faroe", "Atlantic/Jan_Mayen", "Atlantic/Madeira", "Atlantic/Reykjavik", "Atlantic/South_Georgia",
    "Atlantic/St_Helena", "Atlantic/Stanley", "Australia/ACT", "Australia/Adelaide", "Australia/Brisbane", "Australia/Broken_Hill",
    "Australia/Canberra", "Australia/Currie", "Australia/Darwin", "Australia/Eucla", "Australia/Hobart", "Australia/LHI",
    "Australia/Lindeman", "Australia/Lord_Howe", "Australia/Melbourne", "Australia/NSW", "Australia/North", "Australia/Perth",
    "Australia/Queensland", "Australia/South", "Australia/Sydney", "Australia/Tasmania", "Australia/Victoria", "Australia/West",
    "Australia/Yancowinna", "Brazil/Acre", "Brazil/DeNoronha", "Brazil/East", "Brazil/West", "CET", "CST6CDT", "Canada/Atlantic",
    "Canada/Central", "Canada/Eastern", "Canada/Mountain", "Canada/Newfoundland", "Canada/Pacific", "Canada/Saskatchewan",
    "Canada/Yukon", "Chile/Continental", "Chile/EasterIsland", "Cuba", "EET", "EST", "EST5EDT", "Egypt", "Eire", "Etc/GMT",
    "Etc/GMT+0", "Etc/GMT+1", "Etc/GMT+10", "Etc/GMT+11", "Etc/GMT+12", "Etc/GMT+2", "Etc/GMT+3", "Etc/GMT+4", "Etc/GMT+5",
    "Etc/GMT+6", "Etc/GMT+7", "Etc/GMT+8", "Etc/GMT+9", "Etc/GMT-0", "Etc/GMT-1", "Etc/GMT-10", "Etc/GMT-11", "Etc/GMT-12",
    "Etc/GMT-13", "Etc/GMT-14", "Etc/GMT-2", "Etc/GMT-3", "Etc/GMT-4", "Etc/GMT-5", "Etc/GMT-6", "Etc/GMT-7", "Etc/GMT-8",
    "Etc/GMT-9", "Etc/GMT0", "Etc/Greenwich", "Etc/UCT", "Etc/UTC", "Etc/Universal", "Etc/Zulu", "Europe/Amsterdam", "Europe/Andorra",
    "Europe/Astrakhan", "Europe/Athens", "Europe/Belfast", "Europe/Belgrade", "Europe/Berlin", "Europe/Bratislava", "Europe/Brussels",
    "Europe/Bucharest", "Europe/Budapest", "Europe/Busingen", "Europe/Chisinau", "Europe/Copenhagen", "Europe/Dublin", "Europe/Gibraltar",
    "Europe/Guernsey", "Europe/Helsinki", "Europe/Isle_of_Man", "Europe/Istanbul", "Europe/Jersey", "Europe/Kaliningrad", "Europe/Kiev",
    "Europe/Kirov", "Europe/Lisbon", "Europe/Ljubljana", "Europe/London", "Europe/Luxembourg", "Europe/Madrid", "Europe/Malta",
    "Europe/Mariehamn", "Europe/Minsk", "Europe/Monaco", "Europe/Moscow", "Europe/Oslo", "Europe/Paris", "Europe/Podgorica", "Europe/Prague",
    "Europe/Riga", "Europe/Rome", "Europe/Samara", "Europe/San_Marino", "Europe/Sarajevo", "Europe/Saratov", "Europe/Simferopol",
    "Europe/Skopje", "Europe/Sofia", "Europe/Stockholm", "Europe/Tallinn", "Europe/Tirane", "Europe/Tiraspol", "Europe/Ulyanovsk",
    "Europe/Uzhgorod", "Europe/Vaduz", "Europe/Vatican", "Europe/Vienna", "Europe/Vilnius", "Europe/Volgograd", "Europe/Warsaw",
    "Europe/Zagreb", "Europe/Zaporozhye", "Europe/Zurich", "GB", "GB-Eire", "GMT", "GMT+0", "GMT-0", "GMT0", "Greenwich", "Hongkong",
    "Iceland", "Indian/Antananarivo", "Indian/Chagos", "Indian/Christmas", "Indian/Cocos", "Indian/Comoro", "Indian/Kerguelen",
    "Indian/Mahe", "Indian/Maldives", "Indian/Mauritius", "Indian/Mayotte", "Indian/Reunion", "Iran", "Israel", "Jamaica", "Japan",
    "Kwajalein", "Libya", "MET", "MST", "MST7MDT", "Mexico/BajaNorte", "Mexico/BajaSur", "Mexico/General", "NZ", "NZ-CHAT", "Navajo",
    "PRC", "PST8PDT", "Pacific/Apia", "Pacific/Auckland", "Pacific/Bougainville", "Pacific/Chatham", "Pacific/Chuuk", "Pacific/Easter",
    "Pacific/Efate", "Pacific/Enderbury", "Pacific/Fakaofo", "Pacific/Fiji", "Pacific/Funafuti", "Pacific/Galapagos", "Pacific/Gambier",
    "Pacific/Guadalcanal", "Pacific/Guam", "Pacific/Honolulu", "Pacific/Johnston", "Pacific/Kiritimati", "Pacific/Kosrae", "Pacific/Kwajalein",
    "Pacific/Majuro", "Pacific/Marquesas", "Pacific/Midway", "Pacific/Nauru", "Pacific/Niue", "Pacific/Norfolk", "Pacific/Noumea",
    "Pacific/Pago_Pago", "Pacific/Palau", "Pacific/Pitcairn", "Pacific/Pohnpei", "Pacific/Ponape", "Pacific/Port_Moresby", "Pacific/Rarotonga",
    "Pacific/Saipan", "Pacific/Samoa", "Pacific/Tahiti", "Pacific/Tarawa", "Pacific/Tongatapu", "Pacific/Truk", "Pacific/Wake", "Pacific/Wallis",
    "Pacific/Yap", "Poland", "Portugal", "ROC", "ROK", "Singapore", "Turkey", "UCT", "US/Alaska", "US/Aleutian", "US/Arizona", "US/Central",
    "US/East-Indiana", "US/Eastern", "US/Hawaii", "US/Indiana-Starke", "US/Michigan", "US/Mountain", "US/Pacific", "US/Samoa", "UTC",
    "Universal", "W-SU", "WET", "Zulu"
};
        }

        private async Task<string> GetTimeZoneByIpAsync(string ip)
        {
            // Replace `ip` with your actual IP or leave it empty for the current IP's time zone
            var apiUrl = $"http://worldtimeapi.org/api/ip/{ip}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                // Deserialize and return timezone data
                var timeZoneData = JsonConvert.DeserializeObject<TimeZoneResponse>(result);
                return timeZoneData.TimeZone;
            }

            return null;
        }
    }
}