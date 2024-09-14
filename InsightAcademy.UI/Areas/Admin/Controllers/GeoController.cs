using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace InsightAcademy.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GeoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        private readonly IGeoService _geoService;

        public GeoController(IWebHostEnvironment env, AppDbContext context, IGeoService geoService)
        {
            _context = context;
            _env = env;
            _geoService = geoService;
        }

        public async Task<IActionResult> SaveCountries()
        {
            var filePath = Path.Combine(_env.WebRootPath, "json/countries.json");
            var json = System.IO.File.ReadAllText(filePath);
            var countries = JsonConvert.DeserializeObject<List<CountryDto>>(json);
            foreach (var country in countries)
            {
                var countryObj = new Country
                {
                    Name = country.name,
                    CountryCode = country.iso3,
                };
                _context.Countries.Add(countryObj);
                await _context.SaveChangesAsync();
                foreach (var city in country.cities)
                {
                    var cityObj = new InsightAcademy.DomainLayer.Entities.City
                    {
                        Name = city.name,
                        CountryId = countryObj.Id,
                    };
                    _context.City.Add(cityObj);
                    await _context.SaveChangesAsync();
                }
            }
            return View(countries);
        }

        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _geoService.GetAllCountries();
            return Json(countries);
        }

        public async Task<IActionResult> GetCitiesById(long id)
        {
            var cities = await _geoService.GetCitiesByCountryId(id);
            return Json(cities);
        }
    }
}