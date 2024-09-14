using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Subject : BaseEntity
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Subject Name")]
        public string Name { get; set; }

        [StringLength(500)]
        public string Tags { get; set; }
    }
}