using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IDashBoard
    {
        Task<DashBoardDto> GetDashBoardData();

        Task<bool> BlockTutor(string userId);

        Task<bool> UnblockTutor(string userId);

        Task<IEnumerable<Tutor>> GetAllBlockTutorList();

        Task ApproveSubject(string subjectName, string tags);

        Task<IEnumerable<Booking>> RecentTransactions();

        Task<decimal> GetBookingPayment(Guid bookingId);
    }
}