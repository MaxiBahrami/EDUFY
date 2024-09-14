using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Dtos
{
    public class NewSubjectRequestDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Category { get; set; }
        public bool Status { get; set; }
    }
}