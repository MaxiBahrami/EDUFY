// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Text;
using InsightAcademy.ApplicationLayer.Services;

using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace InsightAcademy.UI.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _applicationEmailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService applicationEmailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationEmailSender = applicationEmailSender;
            _httpContextAccessor = httpContextAccessor;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {

            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            string profileLink = $"{baseUrl}/Tutor/PersonalDetails";
            string courseLink = $"{baseUrl}/Courses/Index";
            string teachertText = $"Dear Educator,\r\n\r\nThank you for choosing our platform for your teaching journey. We're excited to have you on board.\r\n\r\nTo complete your profile and enrich your teaching presence, please use the following link: {profileLink}. This will direct you to the page where you can add more details about yourself.\r\n\r\nAdditionally, you can explore all available courses ${courseLink}. Dive into our diverse offerings and see how you can further engage with our community.\r\n\r\nWelcome aboard, and we look forward to your contributions!\r\n";
            string studentText = "Dear Student,\r\n\r\nThank you for choosing our platform. We're committed to providing you with a satisfying and enriching learning experience.\r\n\r\nGet started by exploring our comprehensive list of teachers and courses [here](#). Discover the paths you can take on your educational journey with us.\r\n\r\nWe're thrilled to have you with us. Here's to your success and growth!";

            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }


            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                StatusMessage = "Thank you for confirming your email.";
                var role = await _userManager.GetRolesAsync(user);
                if (role[0] == Roles.Teacher)
                {
                    await _applicationEmailSender.SendWelcomeEmail(user.FullName, user.Email, teachertText);
                }
                else
                {
                    await _applicationEmailSender.SendWelcomeEmail(user.FullName, user.Email, studentText);
                }
            }
            else
            {
                StatusMessage = "Error confirming your email.";
            }

            return Page();
        }
    }
}
