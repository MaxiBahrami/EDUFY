using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class GeoService : IGeoService
    {
        private IGeo _geo;

        public GeoService(IGeo geo)
        {
            _geo = geo;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _geo.GetAllCountries();
        }

        public async Task<List<InsightAcademy.DomainLayer.Entities.City>> GetCitiesByCountryId(long Id)
        {
            return await _geo.GetCitiesByCountryId(Id);
        }

        public async Task<DomainLayer.Entities.City> GetCityById(long id)
        {
            return await _geo.GetCityById(id);
        }

        public async Task<Country> GetCountryById(long id)
        {
            return await _geo.GetCountryById(id);
        }
    }
}