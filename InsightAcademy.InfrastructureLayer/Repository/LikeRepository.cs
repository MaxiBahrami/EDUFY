using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class LikeRepository : ILike
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddLike(Guid tutorId)
        {
            var UserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var like = new Like
            {
                TutorId = tutorId,
                ApplicationUserId = UserId,
            };
            await _context.Like.AddAsync(like);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }

            return true;
        }

        public async Task<bool> DeleteLike(Guid tutorId)
        {
            if (tutorId == null)
            {
                return false;
            }
            var resut = await _context.Like.FirstOrDefaultAsync(x => x.TutorId == tutorId);
            _context.Like.Remove(resut);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Like>> GetAllLike()
        {
            var UserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var result = await _context.Like.Include(x => x.Tutor.TutorSubject).Include(x => x.Tutor).ThenInclude(x => x.TutorSubject).Include(x => x.Tutor).ThenInclude(x => x.ApplicationUser).Where(x => x.ApplicationUserId == UserId).ToListAsync();
            //var result = await _context.Like.Include(z=>z.Tutor).ThenInclude(z=>z.ApplicationUser).Include(z=>z.Tutor).ThenInclude(z=>z.TutorSubject).ThenInclude(z=>z.Subject)
               
            //    .Where(x => x.ApplicationUserId == UserId).ToListAsync();

            var result = await _context.Like
    .Where(x => x.ApplicationUserId == UserId)
    .Include(like => like.Tutor)
        .ThenInclude(tutor => tutor.ApplicationUser)
    .Include(like => like.Tutor)
        .ThenInclude(tutor => tutor.TutorSubject)
            .ThenInclude(tutorSubject => tutorSubject.Subject)
    .ToListAsync();


            return result;
        }

        public async Task<string> AddTeacherToLike(string username)
        {
            var UserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var tutor = await _context.Tutor.FirstOrDefaultAsync(z => z.UserName == username);

            Like newLike = new Like()
            {
                TutorId = tutor.Id,
                ApplicationUserId = UserId,
                CreatedDateTime = DateTime.Now,
            };

            await _context.Like.AddAsync(newLike);
            var Id = await _context.SaveChangesAsync();
            return Id.ToString();
        }

        public async Task<bool> IfTeacherExist(string userName)
        {
            return await _context.Tutor.AnyAsync(z => z.UserName == userName);
        }
    }
}