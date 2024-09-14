using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.IService
{
    public interface IAvailabilityService
    {
        Task<bool> AddOrUpdateAvailability(AvailabilityModel availability);

        Task<string> BookSlot(Guid tutorId, string userId, string bookingDate, string slot);

        Task<IEnumerable<Booking>> GetBookedSlots(Guid tutorId, DateTime dateTime);

        Task<IEnumerable<Booking>> GetMyBookings(string tutorId);

        Task<AvailabilityModel> GetMySchedule(string tutorId);

        Task<IEnumerable<Availability>> GetTutorSlots(Guid tutorId, string date, string studentIANATimeZone);
    }
}