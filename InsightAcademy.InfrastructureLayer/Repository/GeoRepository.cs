using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class GeoRepository : IGeo
    {
        public readonly AppDbContext _context;

        public GeoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _context.Countries.OrderBy(z => z.Name).ToListAsync();
        }

        public async Task<List<DomainLayer.Entities.City>> GetCitiesByCountryId(long Id)
        {
            var citiesInCountry = await _context.City.Where(x => x.CountryId == Id).OrderBy(z => z.Name).ToListAsync();
            return citiesInCountry;
        }

        public async Task<DomainLayer.Entities.City> GetCityById(long id)
        {
            return await _context.City.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Country> GetCountryById(long id)
        {
            return await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}