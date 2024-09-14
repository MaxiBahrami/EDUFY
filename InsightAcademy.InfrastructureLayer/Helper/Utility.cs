using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Helper
{
    public static class Utility
    {
        public static TimeSpan ConvertToTimeSpan(string timeString)
        {
            DateTime time;

            // Use DateTime.TryParseExact to handle AM/PM and 12-hour formats
            if (DateTime.TryParseExact(timeString, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                return time.TimeOfDay;  // Extract the TimeSpan (time of day)
            }
            else
            {
                throw new FormatException("Invalid time format");
            }
        }
    }
}