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
    public class ReviewRepository : IReview
    {
        public readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public ReviewRepository(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _context = context;
        }

        public async Task<bool> AddReview(string feedBack, int rating, Guid tutorId)
        {
            var UserId = _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Review review = new Review();
            if (UserId != null && tutorId != null)
            {
                review.StudentId = UserId;
                review.TutorId = tutorId;
                review.Feedback = feedBack;
                review.Rating = rating;
                review.CreatedDateTime = DateTime.Now;
                await _context.Review.AddAsync(review);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Review>> FilterReviews(double rating, string order, Guid tutorId)
        {
            IQueryable<Review> query = _context.Review.Include(z => z.Student).Where(z => z.TutorId == tutorId);

            if (rating > 0)
            {
                query = query.Where(z => z.Rating == rating);
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (order.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(z => z.CreatedDateTime);
                }
                else if (order.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(z => z.CreatedDateTime);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            var UserId = _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var ReviewList = await _context.Review.Where(x => x.StudentId == UserId).ToListAsync();
            return ReviewList;
        }
    }
}