using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Category:BaseEntity
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}
