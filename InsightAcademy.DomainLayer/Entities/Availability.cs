using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Availability : BaseEntity
    {
        public string TutorId { get; set; }
        public string DayOfWeek { get; set; } // e.g., "Monday", "Tuesday", etc.
        public TimeSpan StartTime { get; set; } // Use TimeSpan to represent time
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}