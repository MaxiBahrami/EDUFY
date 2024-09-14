using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Dtos
{
    public class RecepientsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RecepientId { get; set; }
        public string SenderId { get; set; }
        public string ImageUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTime TimeSpan { get; set; }
    }
}