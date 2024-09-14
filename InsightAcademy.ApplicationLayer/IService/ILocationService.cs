using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.IService
{
	public interface ILocationService
	{
		Task<List<Country>> GetCountris();
	}
}