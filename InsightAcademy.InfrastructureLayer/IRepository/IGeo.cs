using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IGeo
    {
        Task<List<Country>> GetAllCountries();

        Task<List<InsightAcademy.DomainLayer.Entities.City>> GetCitiesByCountryId(long Id);

        Task<Country> GetCountryById(long id);

        Task<DomainLayer.Entities.City> GetCityById(long id);
    }
}