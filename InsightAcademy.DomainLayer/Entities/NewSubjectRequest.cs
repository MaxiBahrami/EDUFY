using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
	public class NewSubjectRequest : BaseEntity
	{
		public string Subject { get; set; }
		public string Category { get; set; }
		public string UserId { get; set; }
		public bool IsApproved { get; set; }

		public ApplicationUser ApplicationUser { get; set; }
	}
}