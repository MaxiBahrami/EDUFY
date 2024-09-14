using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class UserRepository : IUser
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> GetTutorId(string userId)
        {
            var tutor = await _context.Tutor.FirstOrDefaultAsync(z => z.ApplicationUserId == userId);
            if (tutor != null)
            {
                return tutor.Id;
            }
            return Guid.Empty;
        }

        //public async Task<ApplicationUser> GetUserById(string userId)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(z => z.Id == userId);
        //    if (user != null)
        //    {
        //        return user;
        //    }
        //    return null;
        //}
        public async Task<string> GetTutorProfileImage(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(z => z.Id == userId);
            return user?.ProfileImageUrl;
        }

        public async Task<bool> UploadProfileImage(string userId, string imageUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(z => z.Id == userId);
            user.ProfileImageUrl = imageUrl;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ApplicationUser> GetUserDetailById(string userId)
        {
            var userDetails = await _context.Users.FindAsync(userId);
            return userDetails;
        }

        public async Task<ApplicationUser> GetMyProfile(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(z => z.Id == userId);
        }

        public async Task<bool> UpdateProfile(string userId, ApplicationUser user)
        {
            var userObj = await _context.Users.FirstOrDefaultAsync(z => z.Id == userId);

            if (userObj != null)
            {
                userObj.FirstName = user.FirstName;
                userObj.LastName = user.LastName;
                userObj.Website = user.Website;
                userObj.StreetAdress = user.StreetAdress;
                userObj.City = user.City;
                userObj.Country = user.Country;
                if (user.UserImage != null)
                {
                    userObj.ProfileImageUrl = user.ProfileImageUrl;
                }
                else
                {
                    userObj.ProfileImageUrl = userObj.ProfileImageUrl;
                }
                _context.Users.Update(userObj);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> BlockStudent(string userId)
        {
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (student != null)
            {
                student.Status = true;

                _context.Users.Update(student);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UnblockStudent(string userId)
        {
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (student != null)
            {
                student.Status = false;

                _context.Users.Update(student);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task UpdateTimeZone(string timeZone, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            user.TimeZone = timeZone;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetUserTimeZone(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            return user.TimeZone ?? "";
        }

        public async Task<List<Booking>> GetMyBookings(string userId)
        {
            return await _context.Booking.Include(z => z.Tutor).Where(z => z.StudentId.ToString() == userId).ToListAsync();
        }

        public async Task UpdateGoogleMeetLink(string googleMeetLink, Guid tutorId)
        {
            var tutor = await _context.Tutor.FirstOrDefaultAsync(z => z.Id == tutorId);
            tutor.GoogleMeetLink = googleMeetLink;

            _context.Tutor.Update(tutor);
            await _context.SaveChangesAsync();
        }
    }
}