using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Education : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Degree { get; set; }
        [Required]
        [StringLength(50)]
        public string University { get; set; }
        [Required]
        [StringLength(50)]
        public string Location { get; set; }
  
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; }
        [NotMapped]
        public string StartDateString { get; set; }= DateTime.Now.ToString("yyyy-MM-dd");
        [NotMapped]
        public string EndDateString { get; set; }
    }
}
