using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
	public class StudentService : IStudentService
	{
		private IStudent _student;

		public StudentService(IStudent student)
		{
			_student = student;
		}

		public async Task<List<ApplicationUser>> GetLatestStudent()
		{
			return await _student.GetLatestStudent();
		}

		public async Task<List<ApplicationUser>> GetStudentList()
		{
			return await _student.GetStudentList();
		}

		public async Task<bool> EditStudentByAdmin(ApplicationUser user)
		{
			return await _student.EditStudentByAdmin(user);
		}

		public async Task<ApplicationUser> GetStudentById(Guid studentId)
		{
			return await _student.GetStudentById(studentId);
		}
	}
}