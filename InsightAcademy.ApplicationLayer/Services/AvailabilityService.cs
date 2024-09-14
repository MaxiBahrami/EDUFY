using InsightAcademy.ApplicationLayer.Helper;
using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Models;
using InsightAcademy.InfrastructureLayer.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ITutorService _tutorService;
        private readonly IEmailService _emaiService;

        public AvailabilityService(ITutorService tutorService, IAvailabilityRepository availabilityRepository, IHttpContextAccessor httpContextAccessor, IUserService userService, IEmailService emaiService)
        {
            _availabilityRepository = availabilityRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _tutorService = tutorService;
            _emaiService = emaiService;
        }

        public async Task<bool> AddOrUpdateAvailability(AvailabilityModel availability)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tutorId = await _userService.GetTutorId(userId);
            await _userService.UpdateTimeZone(availability.TimeZone, userId);
            await _userService.UpdateGoogleMeetLink(availability.GoogleMeetLink, tutorId);
            // Helper method to add or update availability
            async Task ProcessAvailabilityAsync(string dayOfWeek, IEnumerable<TimeSlot> slots)
            {
                var existingRecords = await _availabilityRepository.GetAvailabilitiesByDayOfWeek(tutorId.ToString(), dayOfWeek);

                var slotSet = new HashSet<(TimeSpan StartTime, TimeSpan EndTime)>(
                    slots.Select(s => (s.StartTime, s.EndTime))
                );

                var recordsToDelete = existingRecords
                                    .Where(record => !slotSet.Contains((record.StartTime, record.EndTime)))
                                    .ToList();

                foreach (var record in recordsToDelete)
                {
                    await _availabilityRepository.DeleteAvailability(record.Id);
                }
                foreach (var slot in slots)
                {
                    var existingAvailability = await _availabilityRepository.GetAvailability(tutorId.ToString(), dayOfWeek, slot.StartTime, slot.EndTime);

                    if (existingAvailability != null)
                    {
                        // Update existing availability
                        existingAvailability.StartTime = slot.StartTime;
                        existingAvailability.EndTime = slot.EndTime;
                        existingAvailability.IsActive = true; // or false depending on your requirement

                        await _availabilityRepository.UpdateAvailability(existingAvailability);
                    }
                    else
                    {
                        // Add new availability
                        var newAvailability = new Availability()
                        {
                            TutorId = tutorId.ToString(),
                            DayOfWeek = dayOfWeek,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime,
                            IsActive = true
                        };

                        await _availabilityRepository.AddAvailability(newAvailability);
                    }
                }
            }

            // Process availability for each day of the week
            if (availability.Monday.Any())
            {
                await ProcessAvailabilityAsync("Monday", availability.Monday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Monday", userId);
            }
            if (availability.Tuesday.Any())
            {
                await ProcessAvailabilityAsync("Tuesday", availability.Tuesday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Tuesday", userId);
            }

            if (availability.Wednesday.Any())
            {
                await ProcessAvailabilityAsync("Wednesday", availability.Wednesday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Wednesday", userId);
            }

            if (availability.Thursday.Any())
            {
                await ProcessAvailabilityAsync("Thursday", availability.Thursday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Thursday", userId);
            }

            if (availability.Friday.Any())
            {
                await ProcessAvailabilityAsync("Friday", availability.Friday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Friday", userId);
            }

            if (availability.Saturday.Any())
            {
                await ProcessAvailabilityAsync("Saturday", availability.Saturday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Saturday", userId);
            }

            if (availability.Sunday.Any())
            {
                await ProcessAvailabilityAsync("Sunday", availability.Sunday);
            }
            else
            {
                _availabilityRepository.MarkDayOff("Sunday", userId);
            }

            return true;
        }

        public async Task<string> BookSlot(Guid tutorId, string studentId, string bookingDate, string slot)
        {
            var bookingId = await _availabilityRepository.BookSlot(tutorId, new Guid(studentId), bookingDate, slot);

            return bookingId;
        }

        public Task<IEnumerable<Booking>> GetBookedSlots(Guid tutorId, DateTime dateTime)
        {
            return _availabilityRepository.GetBookedSlots(tutorId, "", dateTime);
        }

        public Task<IEnumerable<Booking>> GetMyBookings(string tutorId)
        {
            return _availabilityRepository.GetMyBookings(tutorId);
        }

        public async Task<AvailabilityModel> GetMySchedule(string tutorId)
        {
            AvailabilityModel availabilityModel = new AvailabilityModel();
            var schedule = await _availabilityRepository.GetMySchedule(tutorId);

            foreach (var item in schedule)
            {
                if (item.DayOfWeek == "Monday")
                {
                    availabilityModel.Monday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Tuesday")
                {
                    availabilityModel.Tuesday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Wednesday")
                {
                    availabilityModel.Wednesday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Thursday")
                {
                    availabilityModel.Thursday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Friday")
                {
                    availabilityModel.Friday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Saturday")
                {
                    availabilityModel.Saturday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
                if (item.DayOfWeek == "Sunday")
                {
                    availabilityModel.Sunday.Add(new TimeSlot() { StartTime = item.StartTime, EndTime = item.EndTime, IsActive = item.IsActive });
                }
            }
            return availabilityModel;
        }

        public async Task<IEnumerable<Availability>> GetTutorSlots(Guid tutorId, string date, string studentIANATimeZone)
        {
            var day = Convert.ToDateTime(date).DayOfWeek;

            var timeTables = await _availabilityRepository.GetTutorSlots(tutorId, day.ToString(), Convert.ToDateTime(date));
            var tutor = await _tutorService.GetTutor(tutorId);
            var tutorTimeZone = await _userService.GetUserTimeZone(tutor.ApplicationUserId);
            return Utility.MakeSlots(studentIANATimeZone, tutorTimeZone, timeTables);
        }
    }
}