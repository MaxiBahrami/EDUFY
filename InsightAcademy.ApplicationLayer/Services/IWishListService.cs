using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public interface IWishListService
    {
      
        Task<bool> AddWishList(Guid tutorId);
        Task<bool> DeleteWishList(Guid tutorId);

        Task<IEnumerable<WishList>> GetAllWishList();
        Task<bool> IsAlreadyExist(string userId, Guid tutorId);
    }
}
