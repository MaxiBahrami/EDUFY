using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class TutorSubject : BaseEntity
    {
        public Guid TutorId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid CategoryId { get; set; }

        public string Tags { get; set; }

        public Tutor Tutor { get; set; }
        public Subject Subject { get; set; }
        public Category Category { get; set; }
    }
}