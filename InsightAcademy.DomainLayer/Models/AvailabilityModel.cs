using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Models
{
    public class AvailabilityModel
    {
        public AvailabilityModel()
        {
            Monday = new List<TimeSlot>();
            Tuesday = new List<TimeSlot>();
            Wednesday = new List<TimeSlot>();
            Thursday = new List<TimeSlot>();
            Friday = new List<TimeSlot>();
            Saturday = new List<TimeSlot>();
            Sunday = new List<TimeSlot>();
        }

        public List<TimeSlot> Monday { get; set; }
        public List<TimeSlot> Tuesday { get; set; }
        public List<TimeSlot> Wednesday { get; set; }
        public List<TimeSlot> Thursday { get; set; }
        public List<TimeSlot> Friday { get; set; }
        public List<TimeSlot> Saturday { get; set; }
        public List<TimeSlot> Sunday { get; set; }
        public string TimeZone { get; set; }
        public string GoogleMeetLink { get; set; }
    }

    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}