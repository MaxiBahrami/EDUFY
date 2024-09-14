using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.Helper;
using InsightAcademy.InfrastructureLayer.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Repository
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        public readonly AppDbContext _context;

        public AvailabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAvailability(Availability availability)
        {
            _context.Availability.Add(availability);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddPayment(Payment payment)
        {
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<string> BookSlot(Guid tutorId, Guid studentId, string bookingDate, string slot)
        {
            var StartTime = Utility.ConvertToTimeSpan(slot.Split('-')[0].Trim());
            var EndTime = Utility.ConvertToTimeSpan(slot.Split('-')[1].Trim());

            Booking newBooking = new Booking()
            {
                TutorId = tutorId,
                StudentId = studentId,
                BookingDate = Convert.ToDateTime(bookingDate),
                StartTime = StartTime,
                EndTime = EndTime,
                CreatedDateTime = DateTime.UtcNow,
            };
            _context.Booking.Add(newBooking);
            await _context.SaveChangesAsync();
            return newBooking.Id.ToString();
        }

        public async Task DeleteAvailability(Guid id)
        {
            var availability = _context.Availability.Find(id);
            _context.Availability.Remove(availability);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTutorSchedule(string userId)
        {
            var availabilityToUpdate = await _context.Availability
                                      .Where(e => e.TutorId == userId)
                                      .ToListAsync();

            foreach (var availability in availabilityToUpdate)
            {
                availability.IsActive = false;
                _context.Availability.Update(availability);
            }

            // Save changes to the database
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DoesScheduleExist(string userId)
        {
            var schedule = await _context.Availability.Where(z => z.TutorId == userId).AsNoTracking().ToListAsync();
            _context.Availability.RemoveRange(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Availability>> GetAvailabilitiesByDayOfWeek(string userId, string dayOfWeek)
        {
            return await _context.Availability.Where(z => z.TutorId == userId && z.DayOfWeek == dayOfWeek).AsNoTracking().ToListAsync();
        }

        public async Task<Availability> GetAvailability(string userId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.Availability.FirstOrDefaultAsync(z => z.TutorId == userId && z.DayOfWeek == dayOfWeek && z.StartTime == startTime && z.EndTime == endTime);
        }

        public async Task<IEnumerable<Availability>> GetMySchedule(string tutorId)
        {
            return await _context.Availability.Where(z => z.TutorId == tutorId).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Availability>> GetTutorSlots(Guid tutorId, string day, DateTime bookingDate)
        {
            return await _context.Availability.Where(z => z.TutorId == tutorId.ToString() && z.DayOfWeek == day).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookedSlots(Guid tutorId, string day, DateTime bookingDate)
        {
            return await _context.Booking.Where(z => z.TutorId == tutorId && z.BookingDate.Date == bookingDate.Date).AsNoTracking().ToListAsync();
        }

        public void MarkDayOff(string day, string userId)
        {
            _context.Availability.Where(z => z.TutorId == userId && z.DayOfWeek == day).ExecuteUpdate(s => s.SetProperty(p => p.IsActive, false));
        }

        public async Task UpdateAvailability(Availability existingAvailability)
        {
            _context.Availability.Update(existingAvailability);
            await _context.SaveChangesAsync();
        }

        public async Task<Booking> UpdateBookingStatus(Guid bookingId)
        {
            var booking = await _context.Booking.FirstOrDefaultAsync(z => z.Id == bookingId);
            booking.Status = "Paid";
            _context.Booking.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetMyBookings(string tutorId)
        {
            return await _context.Booking.Where(z => z.TutorId.ToString() == tutorId && z.Status == "Paid").AsNoTracking().ToListAsync();
        }
    }
}