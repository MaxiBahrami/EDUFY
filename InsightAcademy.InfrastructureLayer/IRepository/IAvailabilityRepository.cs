using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.IRepository
{
    public interface IAvailabilityRepository
    {
        Task<bool> AddAvailability(Availability availability);

        Task AddPayment(Payment payment);

        Task<string> BookSlot(Guid tutorId, Guid studentId, string bookingDate, string slot);

        Task DeleteAvailability(Guid id);

        Task<IEnumerable<Booking>> GetBookedSlots(Guid tutorId, string day, DateTime bookingDate);

        Task<bool> DeleteTutorSchedule(string userId);

        Task<bool> DoesScheduleExist(string userId);

        Task<IEnumerable<Availability>> GetAvailabilitiesByDayOfWeek(string userId, string dayOfWeek);

        Task<Availability> GetAvailability(string userId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime);

        Task<IEnumerable<Availability>> GetMySchedule(string userId);

        Task<IEnumerable<Availability>> GetTutorSlots(Guid tutorId, string day, DateTime date);

        void MarkDayOff(string day, string userId);

        Task UpdateAvailability(Availability existingAvailability);

        Task<Booking> UpdateBookingStatus(Guid bookingId);
        Task<IEnumerable<Booking>> GetMyBookings(string tutorId);
    }
}