using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Identity;

public class LockoutMiddleware
{
    private readonly RequestDelegate _next;

    public LockoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userManager = context.RequestServices.GetService<UserManager<ApplicationUser>>();
            var signInManager = context.RequestServices.GetService<SignInManager<ApplicationUser>>();

            var user = await userManager.GetUserAsync(context.User);
            if (user != null && user.Status == true)
            {
                var path = context.Request.Path;
                if (!path.HasValue || (!path.Value.Equals("/Identity/Account/Lockout", StringComparison.OrdinalIgnoreCase) &&
                                      !path.Value.Equals("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) &&
                                      !path.Value.Equals("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.Redirect("/Identity/Account/Lockout");
                    return;
                }
            }
        }

        await _next(context);
    }
}