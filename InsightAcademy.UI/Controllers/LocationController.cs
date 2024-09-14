using InsightAcademy.ApplicationLayer.IService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Text.Json;

using System.Threading.Tasks; // Import for async/await

namespace InsightAcademy.UI.Controllers
{
    public class LocationController : Controller
    {
        private ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<IActionResult> GetCountries()
        {
            var countries = await _locationService.GetCountris();
            return new JsonResult(countries);
        }
    }
}