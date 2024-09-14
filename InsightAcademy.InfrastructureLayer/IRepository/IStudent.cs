using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
	public interface IStudent
	{
		Task<List<ApplicationUser>> GetStudentList();

		Task<List<ApplicationUser>> GetLatestStudent();

		Task<bool> EditStudentByAdmin(ApplicationUser user);

		Task<ApplicationUser> GetStudentById(Guid studentId);
	}
}