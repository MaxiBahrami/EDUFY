using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILike _like;

        public LikeService(ILike like)
        {
            _like = like;
        }

        public async Task<bool> AddLike(Guid tutorId)
        {
            return await _like.AddLike(tutorId);
        }

        public async Task<IEnumerable<Like>> GetAllLike()
        {
            return await _like.GetAllLike();
        }

        public async Task<bool> DeleteLike(Guid tutorId)
        {
            return await _like.DeleteLike(tutorId);
        }

        public async Task<string> AddTeacherToLike(string username)
        {
            if (await _like.IfTeacherExist(username))
            {
                return await _like.AddTeacherToLike(username);
            }

            return "";
        }
    }
}