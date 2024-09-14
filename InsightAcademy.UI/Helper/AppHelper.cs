using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InsightAcademy.UI.Helper
{
    public static class AppHelper
    {
        public static string HideText(string text)
        {
            string formatedText = string.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                if (text?.Length <= 4)
                    formatedText = text; // If text is 4 characters or less, don't hide anything
                else
                    formatedText = text.Substring(0, 4) + new string('*', text.Length - 4); // Replace remaining characters with *
            }
            return formatedText;
        }

        public static string GetLastOnlineText(DateTime lastOnline)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(lastOnline);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", Math.Abs(timeSpan.Seconds));
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    System.String.Format("about {0} minutes ago", Math.Abs(timeSpan.Minutes)) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    System.String.Format("about {0} hours ago", Math.Abs(timeSpan.Hours)) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    System.String.Format("about {0} days ago", Math.Abs(timeSpan.Days)) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    System.String.Format("about {0} months ago", Math.Abs(timeSpan.Days / 30)) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    System.String.Format("about {0} years ago", Math.Abs(timeSpan.Days / 365)) :
                    "about a year ago";
            }

            return result;
        }

        public static string UTCToLocal(DateTime lastOnline, string countryName)
        {
            string result = string.Empty;

            DateTime utcTime = lastOnline;

            // Get user's time zone based on country code
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneId(countryName));

            // Convert UTC time to user's local time
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, userTimeZone);

            var timeSpan = DateTime.Now.Subtract(localTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    System.String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    System.String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    System.String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    System.String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    System.String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }

        private static string GetTimeZoneId(string countryCode)
        {
            switch (countryCode.ToUpper())
            {
                case "PAKISTAN":
                    return "Pakistan Standard Time"; // Change to the appropriate time zone for the US
                case "Sewden":
                    return "GMT Standard Time"; // Change to the appropriate time zone for the UK
                                                // Add more cases for other countries as needed
                default:
                    return "UTC"; // Default to UTC if country code is not recognized
            }
        }

        public static double CalculateAverageRating(double totalReview, int totalCount)
        {
            if (totalCount == 0)
            {
                return 0;
            }

            double averageRating = totalReview / totalCount;
            return Math.Ceiling(averageRating * 10) / 10;
        }
    }
}