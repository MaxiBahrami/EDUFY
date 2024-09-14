using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
	public class StudentRepository : IStudent
	{
		public readonly AppDbContext _context;

		public StudentRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<ApplicationUser>> GetStudentList()
		{
			var usersWithRoles = await (from user in _context.Users
										join userRole in _context.UserRoles on user.Id equals userRole.UserId
										join role in _context.Roles on userRole.RoleId equals role.Id
										where role.Name == Roles.Student
										select user).ToListAsync();

			return usersWithRoles;
		}

		public async Task<List<ApplicationUser>> GetLatestStudent()
		{
			int currentMonth = DateTime.Now.Month;
			var usersWithRoles = await (from user in _context.Users
										join userRole in _context.UserRoles on user.Id equals userRole.UserId
										join role in _context.Roles on userRole.RoleId equals role.Id
										where role.Name == Roles.Student && user.CreatedDate.Month == currentMonth
										select user).ToListAsync();

			return usersWithRoles;
		}

		public async Task<bool> EditStudentByAdmin(ApplicationUser user)
		{
			var student = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
			student.City = user.City;
			student.Country = user.Country;
			student.Website = user.Website;
			student.PhoneNumber = user.PhoneNumber;
			_context.Users.Update(student);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<ApplicationUser> GetStudentById(Guid studentId)
		{
			var student = await _context.Users.FirstOrDefaultAsync(x => x.Id == studentId.ToString());
			return student;
		}
	}
}