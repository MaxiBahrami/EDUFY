using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IReview
    {
        Task<bool> AddReview(string feedBack, int rating, Guid tutorId);

        Task<IEnumerable<Review>> FilterReviews(double rating, string order, Guid tutorId);

        Task<IEnumerable<Review>> GetAllReviews();
    }
}