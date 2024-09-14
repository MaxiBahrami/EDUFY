using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Contact: BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string     Email { get; set; }
        public string SkypeId { get; set; }
        public string WhatsappNumber { get; set; }
        public string Website { get; set; }
        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; }
    }
}
