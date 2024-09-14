using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
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
    public class DashBoardRepository : IDashBoard
    {
        public readonly AppDbContext _context;

        public DashBoardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashBoardDto> GetDashBoardData()
        {
            int currentMonth = DateTime.Now.Month;
            DashBoardDto dashBoard = new DashBoardDto();

            dashBoard.AllStudent = await (from user in _context.Users
                                          join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                          join role in _context.Roles on userRole.RoleId equals role.Id
                                          where role.Name == Roles.Student
                                          select user).CountAsync();

            dashBoard.LatestStudent = await (from user in _context.Users
                                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                             join role in _context.Roles on userRole.RoleId equals role.Id
                                             where role.Name == Roles.Student && user.CreatedDate.Month == currentMonth
                                             select user).CountAsync();
            dashBoard.AllTutor = await (from user in _context.Users
                                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                        join role in _context.Roles on userRole.RoleId equals role.Id
                                        where role.Name == Roles.Teacher
                                        select user).CountAsync();

            dashBoard.Transactions = await _context.Payment.Where(z => z.Status == "Completed").SumAsync(z => z.Amount);

            return dashBoard;
        }

        public async Task<bool> BlockTutor(string userId)
        {
            var tutor = await _context.Tutor.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);

            if (tutor != null)
            {
                tutor.IsBlock = false;

                _context.Tutor.Update(tutor);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UnblockTutor(string userId)
        {
            var tutor = await _context.Tutor.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);

            if (tutor != null)
            {
                tutor.IsBlock = true;

                _context.Tutor.Update(tutor);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Tutor>> GetAllBlockTutorList()
        {
            var blockTutorList = await _context.Tutor.Where(x => x.IsBlock == true)
        .Include(tutor => tutor.Contact)
        .Include(tutor => tutor.ApplicationUser)
        .Include(tutor => tutor.WishLists)
        .Include(tutor => tutor.Media)
        .Include(tutor => tutor.TutorSubject)
        .Include(tutor => tutor.Reviews)
        .ToListAsync();

            return blockTutorList;
        }

        public async Task ApproveSubject(string subjectName, string tags)
        {
            Subject subject = new Subject();
            subject.Name = subjectName;
            subject.Tags = tags;

            _context.Subject.Add(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> RecentTransactions()
        {
            return await _context.Booking.ToListAsync();
        }

        public async Task<decimal> GetBookingPayment(Guid bookingId)
        {
            var payment = await _context.Payment.FirstOrDefaultAsync(z => z.BookingId == bookingId);
            if (payment == null)
            {
                return 0;
            }
            return payment.Amount;
        }
    }
}