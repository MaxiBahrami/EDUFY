using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.ViewModel
{
    public class DashBoardViewModel
    {
        public DashBoardDto DashBoard { get; set; }
        public List<Tutor> Tutor { get; set; }
        public IEnumerable<NewSubjectRequestDto> NewSubjectRequests { get; set; }
        public IEnumerable<RecentTransactionsDto> RecentTransactions { get; set; }
    }
}