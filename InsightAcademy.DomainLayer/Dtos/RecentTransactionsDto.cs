using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Dtos
{
    public class RecentTransactionsDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string TutorName { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentPhone { get; set; }
        public string StudentImage { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }
}