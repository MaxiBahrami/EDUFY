using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    internal class LocationService : ILocationService
    {
        private ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<Country>> GetCountris()
        {
            return await _locationRepository.GetCountris();
        }
    }
}