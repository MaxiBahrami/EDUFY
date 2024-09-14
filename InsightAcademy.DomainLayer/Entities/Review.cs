using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Review : BaseEntity
    {
        public double Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Feedback { get; set; }

        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; }
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }
    }
}