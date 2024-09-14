using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
	public class Country
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string CountryCode { get; set; }
		public ICollection<City> Cities { get; set; }
	}
}