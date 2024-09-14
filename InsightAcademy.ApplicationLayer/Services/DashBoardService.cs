using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class DashBoardService : IDashBoardService
    {
        private IDashBoard _dashBoard;
        private IUserService _userService;
        private ITutor _tutor;

        public DashBoardService(IDashBoard dashBoard, IUserService userService, ITutor tutor)
        {
            _dashBoard = dashBoard;
            _userService = userService;
            _tutor = tutor;
        }

        public async Task<DashBoardDto> GetDashBoardData()
        {
            return await _dashBoard.GetDashBoardData();
        }

        public async Task<bool> BlockTutor(string userId)
        {
            return await _dashBoard.BlockTutor(userId);
        }

        public async Task<bool> UnblockTutor(string userId)
        {
            return await _dashBoard.UnblockTutor(userId);
        }

        public async Task<IEnumerable<Tutor>> GetAllBlockTutorList()
        {
            return await _dashBoard.GetAllBlockTutorList();
        }

        public async Task<IEnumerable<RecentTransactionsDto>> RecentTransactions()
        {
            var bookings = await _dashBoard.RecentTransactions();
            List<RecentTransactionsDto> transactionDto = new List<RecentTransactionsDto>();
            int index = 0;
            foreach (var item in bookings)
            {
                transactionDto.Add(new RecentTransactionsDto()
                {
                    Id = item.Id,
                    Code = ++index,
                    StudentName = _userService.GetUserDetailById(item.StudentId.ToString()).Result.FullName ?? "",
                    StudentImage = _userService.GetUserDetailById(item.StudentId.ToString()).Result.ProfileImageUrl ?? "",
                    TutorName = _tutor.GetTutor(item.TutorId).Result.FullName ?? "",
                    Status = item.Status,
                    StudentEmail = _userService.GetUserDetailById(item.StudentId.ToString()).Result.Email ?? "",
                    StudentPhone = _userService.GetUserDetailById(item.StudentId.ToString()).Result.PhoneNumber ?? "",
                    Amount = await _dashBoard.GetBookingPayment(item.Id),
                });
            }
            return transactionDto;
        }

        public async Task<decimal> GetBookingPayment(Guid bookingId)
        {
            return await _dashBoard.GetBookingPayment(bookingId);
        }
    }
}