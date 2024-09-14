using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.IRepository
{
    public interface ILocationRepository
    {
        Task<List<Country>> GetCountris();
    }
}