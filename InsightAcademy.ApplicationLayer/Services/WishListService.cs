using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class WishListService : IWishListService
    {
       private readonly IWishList _wishList;
        public WishListService(IWishList wishList)
        {
            _wishList = wishList;
                
        }
    
        public async Task<bool> AddWishList(Guid tutorId)
        {
            return await _wishList.AddWishList(tutorId);
           
        }
       public async Task<IEnumerable<WishList>> GetAllWishList()
        {
            
            return await _wishList.GetAllWishList();
        }
      public async  Task<bool> DeleteWishList(Guid tutorId)
        {
            return await _wishList.DeleteWishList(tutorId);
        }

        public async Task<bool> IsAlreadyExist(string userId, Guid tutorId)
        {
            return await _wishList.IsAlreadyExist(userId,tutorId);
        }
    }
}
