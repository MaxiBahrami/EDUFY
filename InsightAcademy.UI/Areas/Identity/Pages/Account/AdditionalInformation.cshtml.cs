using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsightAcademy.UI.Areas.Identity.Pages.Account
{
    public class AdditionalInformationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITutor _tutor;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Role { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string LoginProvider { get; set; }

        [BindProperty]
        public string ProviderKey { get; set; }

        [BindProperty]
        public string DisplayName { get; set; }

        public AdditionalInformationModel(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ITutor tutor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tutor = tutor;
        }

        public void OnGet(string email, string loginProvider, string providerKey, string displayName)
        {
            Email = email;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            DisplayName = displayName;
        }

        public async Task<IActionResult> OnPost()
        {
            var userEmail = Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        FirstName = "",
                        LastName = "",
                        UserName = userEmail,
                        Email = userEmail,
                        EmailConfirmed = true,
                        Gender = Gender,
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, Role);
                    UserLoginInfo userLoginInfo = new UserLoginInfo(LoginProvider, ProviderKey, DisplayName);
                    await _userManager.AddLoginAsync(user, userLoginInfo);

                    if (Role == Roles.Teacher)
                    {
                        Tutor profile = new Tutor() { ApplicationUserId = user.Id, FirstName = user.FirstName, LastName = user.LastName, Country = user.Country, City = user.City, HourlyRate = 0, Introduction = "", Tagline = "", IsVerified = false, UserName = GenerateUsername(user.Email) };
                        await _tutor.AddPersonalDetialAsync(profile);
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return Redirect("/tutor/list");
        }

        private string GenerateUsername(string emailAddress)
        {
            // Split email address by "@" symbol
            string[] parts = emailAddress.Split('@');

            // Use the first part of the email address as the username
            string username = parts[0];

            return username;
        }
    }
}