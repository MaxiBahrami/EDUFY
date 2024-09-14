using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Country>> GetCountris()
        {
            return await _context.Countries.AsNoTracking().OrderBy(z => z.Name).ToListAsync();
        }
    }
}