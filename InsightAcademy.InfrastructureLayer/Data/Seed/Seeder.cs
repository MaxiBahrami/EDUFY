using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace InsightAcademy.InfrastructureLayer.Data.Seed;

public class Seeder : IHostedService
{
    private readonly IServiceProvider _services;

    public Seeder(IServiceProvider services)
    {
        _services = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAndAdmin();
    }

    private async Task SeedRolesAndAdmin()
    {
        string adminEmail = "Admin@gmail.com";
        string adminPassword = "Pa$$w0rd";
        string[] rolesToAdd = { Roles.SuperAdmin, Roles.Teacher, Roles.Student };

        using (var scope = _services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAndUser(adminEmail, adminPassword, rolesToAdd, roleManager, userManager);
            //await SeedCountries(dbContext);
        }
    }

    //private async Task SeedCountries(AppDbContext dbContext)
    //{
    //    if (!dbContext.Countries.Any())
    //    {
    //        var client = new HttpClient();
    //        var request = new HttpRequestMessage(HttpMethod.Get, "https://restcountries.com/v3.1/all");

    //        var response = await client.SendAsync(request);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            // Await the Task and get the actual stream
    //            var stream = await response.Content.ReadAsStreamAsync();
    //            var countriesData = await JsonSerializer.DeserializeAsync<List<CountryDto>>(stream);
    //            List<Country> countries = new List<Country>();
    //            foreach (var countryData in countriesData)
    //            {
    //                countries.Add(new Country() { Name = countryData.name.common, CountryCode = countryData.cca3 });
    //            }

    //            await dbContext.Countries.AddRangeAsync(countries);
    //            await dbContext.SaveChangesAsync();
    //        }
    //    }
    //}

    private static async Task SeedRolesAndUser(string adminEmail, string adminPassword, string[] rolesToAdd, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        foreach (string roleName in rolesToAdd)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = roleName.ToUpper() });
            }
        }

        // Check if SuperAdmin user exists
        var superAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (superAdmin == null)
        {
            // SuperAdmin user doesn't exist, create it
            superAdmin = new ApplicationUser { FirstName = "Admin", LastName = "Admin", UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var createResult = await userManager.CreateAsync(superAdmin, adminPassword);

            if (createResult.Succeeded)
            {
                // Assign SuperAdmin role to the SuperAdmin user
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}