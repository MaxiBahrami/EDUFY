using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUser _user;

        public UserService(IUser user)
        {
            _user = user;
        }

        public async Task<Guid> GetTutorId(string userId)
        {
            return await _user.GetTutorId(userId);
        }

        public async Task<string> GetTutorProfileImage(string userId)
        {
            return await _user.GetTutorProfileImage(userId);
        }

        public async Task<bool> UploadProfileImage(string userId, string imageUrl)
        {
            return await _user.UploadProfileImage(userId, imageUrl);
        }

        public async Task<ApplicationUser> GetUserDetailById(string userId)
        {
            return await _user.GetUserDetailById(userId);
        }

        public async Task<bool> UpdateProfile(string userId, ApplicationUser user)
        {
            return await _user.UpdateProfile(userId, user);
        }

        public async Task<bool> BlockStudent(string userId)
        {
            return await _user.BlockStudent(userId);
        }

        public async Task<bool> UnblockStudent(string userId)
        {
            return await _user.UnblockStudent(userId);
        }

        public async Task UpdateTimeZone(string timeZone, string userId)
        {
            await _user.UpdateTimeZone(timeZone, userId);
        }

        public async Task<string> GetUserTimeZone(string userId)
        {
            return await _user.GetUserTimeZone(userId);
        }

        public async Task<(List<Booking> futureBookings, List<Booking> pastBookings)> GetMyBookings(string userId)
        {
            var currentDate = DateTime.Now;

            var bookings = await _user.GetMyBookings(userId);

            var futureBookings = bookings
    .Where(b => b.BookingDate.Date >= currentDate.Date)
    .OrderBy(b => b.BookingDate) // Optional: Sort by date
    .ToList();

            var pastBookings = bookings
        .Where(b => b.BookingDate.Date < currentDate.Date)
        .OrderByDescending(b => b.BookingDate) // Optional: Sort by date in descending order
        .ToList();

            return (futureBookings, pastBookings);
        }

        public async Task UpdateGoogleMeetLink(string googleMeetLink, Guid tutorId)
        {
            await _user.UpdateGoogleMeetLink(googleMeetLink, tutorId);
        }
    }
}