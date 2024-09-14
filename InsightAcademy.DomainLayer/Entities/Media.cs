using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Media:BaseEntity
    {
        [Required]
        public string Url { get; set; }
        [Required]
        [StringLength(20)]
        public string Type { get; set; } //can be image or video
        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; }
    }
}
