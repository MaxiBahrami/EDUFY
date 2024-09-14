using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public interface IUserService
    {
        Task<Guid> GetTutorId(string userId);

        Task<string> GetTutorProfileImage(string userId);

        Task<bool> UploadProfileImage(string userId, string imageUrl);

        Task<ApplicationUser> GetUserDetailById(string userId);

        Task<bool> UpdateProfile(string userId, ApplicationUser user);

        Task<bool> BlockStudent(string userId);

        Task<bool> UnblockStudent(string userId);

        Task UpdateTimeZone(string timeZone, string userId);

        Task<string> GetUserTimeZone(string userId);

        Task<(List<Booking> futureBookings, List<Booking> pastBookings)> GetMyBookings(string userId);

        Task UpdateGoogleMeetLink(string googleMeetLink, Guid tutorId);
    }
}