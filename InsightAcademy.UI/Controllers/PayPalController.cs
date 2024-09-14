using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using InsightAcademy.UI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsightAcademy.UI.Controllers
{
    public class PayPalController : Controller
    {
        private readonly PayPalClient _payPalClient;
        private readonly IUser _user;
        private readonly IAvailabilityService _availabilityService;
        private readonly ITutorService _tutorService;

        public PayPalController(PayPalClient payPalClient, IUser user, IAvailabilityService availabilityService, ITutorService tutorService)
        {
            _payPalClient = payPalClient;
            _user = user;
            _availabilityService = availabilityService;
            _tutorService = tutorService;
        }

        public async Task<IActionResult> CreatePayment(string studentTimeZone, Guid tutorId, string bookingDate, string slot)
        {
            using (var client = new HttpClient())
            {
                var userId = User.GetUserId();
                var timeZone = await _user.GetUserTimeZone(userId);
                var tutor = await _tutorService.GetTutor(tutorId);
                if (timeZone != studentTimeZone)
                {
                    await _user.UpdateTimeZone(studentTimeZone, userId);
                }
                var bookingId = await _availabilityService.BookSlot(tutorId, userId, bookingDate, slot);

                // Set the PayPal API base URL
                client.BaseAddress = new Uri(_payPalClient.BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

                var payment = new
                {
                    intent = "sale",
                    payer = new { payment_method = "paypal" },
                    transactions = new[]
                    {
                    new {
                        amount = new {
                            total = tutor.HourlyRate,  // Amount to charge
                            currency = "USD"
                        },
                        description = "Your purchase description"
                    }
                },
                    redirect_urls = new
                    {
                        return_url = Url.Action("PaymentSuccess", "Home", new { bookingId = bookingId, amount = tutor.HourlyRate }, Request.Scheme),
                        cancel_url = Url.Action("PaymentCancel", "Home", null, Request.Scheme)
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payment), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/v1/payments/payment", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paymentResult = JsonSerializer.Deserialize<PayPalPaymentResponse>(jsonResponse);

                    var approvalUrl = paymentResult.links.FirstOrDefault(x => x.rel == "approval_url")?.href;
                    if (!string.IsNullOrEmpty(approvalUrl))
                    {
                        return new JsonResult(approvalUrl);
                    }
                }
            }

            return new JsonResult("/PayPal/PaymentFailed");
        }

        private async Task<string> GetAccessTokenAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_payPalClient.BaseUrl);
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_payPalClient.ClientId}:{_payPalClient.ClientSecret}"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.PostAsync("/v1/oauth2/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tokenResult = JsonSerializer.Deserialize<PayPalTokenResponse>(jsonResponse);
                    return tokenResult.access_token;
                }
            }

            return null;
        }
    }
}