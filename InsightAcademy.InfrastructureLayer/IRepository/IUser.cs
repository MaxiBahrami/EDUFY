using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IUser
    {
        Task<Guid> GetTutorId(string userId);

        Task<ApplicationUser> GetUserDetailById(string userId);

        Task<string> GetTutorProfileImage(string userId);

        Task<string> GetLoginUserName(string userId);

        Task<bool> UploadProfileImage(string userId, string imageUrl);

        Task<ApplicationUser> GetMyProfile(string userId);

        Task<bool> UpdateProfile(string userId, ApplicationUser user);

        Task<bool> BlockStudent(string userId);

        Task<bool> UnblockStudent(string userId);

        Task UpdateTimeZone(string timeZone, string userId);

        Task<string> GetUserTimeZone(string userId);

        Task<List<Booking>> GetMyBookings(string userId);

        Task UpdateGoogleMeetLink(string googleMeetLink, Guid tutorId);
    }
}