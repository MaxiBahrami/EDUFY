using InsightAcademy.DomainLayer.Entities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Helper
{
    public static class Utility
    {
        public static IEnumerable<Availability> MakeSlots(string studentIANATimeZone, string tutorIANATimeZone, IEnumerable<Availability> timeTables)
        {
            foreach (var time in timeTables)
            {
                var tutorTimeZone = DateTimeZoneProviders.Tzdb[tutorIANATimeZone];
                var studentTimeZone = DateTimeZoneProviders.Tzdb[studentIANATimeZone];

                var tutorStartTime = new LocalDateTime(2024, 9, 5, time.StartTime.Hours, time.StartTime.Minutes);
                var tutorEndTime = new LocalDateTime(2024, 9, 5, time.EndTime.Hours, time.EndTime.Minutes);

                var tutorStartTimeZoned = tutorTimeZone.AtLeniently(tutorStartTime);
                var tutorEndTimeZoned = tutorTimeZone.AtLeniently(tutorEndTime);

                var tutorStartTimeUTC = tutorStartTimeZoned.ToInstant().InUtc();
                var tutorEndTimeUTC = tutorEndTimeZoned.ToInstant().InUtc();

                // Convert UTC time to student's local time
                var studentStartTimeLocal = tutorStartTimeUTC.WithZone(studentTimeZone);
                var studentEndTimeLocal = tutorEndTimeUTC.WithZone(studentTimeZone);

                time.StartTime = new TimeSpan(studentStartTimeLocal.Hour, studentStartTimeLocal.Minute, studentStartTimeLocal.Second);
                time.EndTime = new TimeSpan(studentEndTimeLocal.Hour, studentEndTimeLocal.Minute, studentEndTimeLocal.Second);
            }
            return timeTables;
        }
    }
}