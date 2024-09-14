using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Dtos
{
    public class DashBoardDto
    {
        public int AllStudent { get; set; }
        public int LatestStudent { get; set; }
        public int AllTutor { get; set; }
        public decimal Transactions { get; set; }
    }
}