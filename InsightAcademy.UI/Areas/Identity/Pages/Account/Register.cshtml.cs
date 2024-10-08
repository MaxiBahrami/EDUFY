﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using DNTCaptcha.Core;
using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using InsightAcademy.UI.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

namespace InsightAcademy.UI.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITutor _tutor;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly FileUploader _fileUploader;
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly IGeoService _geoService;

        public RegisterModel(
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IGeoService geoService,
            ILogger<RegisterModel> logger,
            FileUploader fileUploader,
            IDNTCaptchaValidatorService validatorService,
            ITutor tutor)

        {
            _emailSender = emailSender;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _fileUploader = fileUploader;
            _validatorService = validatorService;
            _tutor = tutor;
            _geoService = geoService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public string Website { get; set; }
            public string StreetAdress { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Role { get; set; }
            public string Gender { get; set; }
            public IFormFile ProfileImage { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            //if (role!="user")
            //{
            //    if (!string.IsNullOrEmpty(role))
            //    {
            //        Input = new InputModel();
            //        Input.Role = role;
            //    }
            //}
            ViewData["Country"] = new SelectList(await _geoService.GetAllCountries(), "Id", "Name");

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (!_validatorService.HasRequestValidCaptchaEntry())
                {
                    TempData["captchaError"] = "Please enter a valid security code.";
                    return Page();
                }
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.Phone;
                user.Website = Input.Website;
                user.City = Input.City;
                user.Country = Input.Country;
                user.Gender = Input.Gender;

                user.ProfileImageUrl = Input.Gender == "male" ? "images/login/man.png" : "images/login/woman.png";
                user.LastOnline = DateTime.Now;
                //upload file
                var uploadResult = await _fileUploader.UploadFile(Input.ProfileImage);
                if (uploadResult.status)
                {
                    user.ProfileImageUrl = uploadResult.url;
                }
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    if (Input.Role == "Teacher")
                    {
                        var rolestatus = await _userManager.AddToRoleAsync(user, Roles.Teacher);
                        if (rolestatus.Succeeded)
                        {
                            //add personal details
                            Tutor profile = new Tutor() { ApplicationUserId = user.Id, FirstName = user.FirstName, LastName = user.LastName, Country = user.Country, City = user.City, HourlyRate = 0, Introduction = "", Tagline = "", IsVerified = false, UserName = GenerateUsername(user.Email) };
                            await _tutor.AddPersonalDetialAsync(profile);
                        }
                        //return Redirect("/Tutor/personaldetails");
                    }
                    if (Input.Role == "Student")
                    {
                        var rolestatus = await _userManager.AddToRoleAsync(user, Roles.Student);
                    }
                    //========================= need to work on email==========================
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
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