using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class ReviewService : IReviewService
    {
        private IReview _review;

        public ReviewService(IReview review)
        {
            _review = review;
        }

        public async Task<bool> AddReview(string feedBack, int rating, Guid tutorId)
        {
            if (rating == 0)
            {
                rating = 1;
            }
            return await _review.AddReview(feedBack, rating, tutorId);
        }

        public async Task<IEnumerable<Review>> FilterReviews(double rating, string order, Guid tutorId)
        {
            return await _review.FilterReviews(rating, order, tutorId);
        }

        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            return await _review.GetAllReviews();
        }
    }
}