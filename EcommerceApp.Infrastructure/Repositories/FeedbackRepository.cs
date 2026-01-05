using EcommerceApp.Application.Interfaces.Feedbacks;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext db;
        public FeedbackRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            var result = await db.Feedbacks.AddAsync(feedback);
            return result.Entity;
        }
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksOfSeller(int sellerId)
        {
            return await db.Feedbacks.AsNoTracking().Where(x => x.SellerId == sellerId).ToListAsync();
        }
        public async Task<decimal> GetAverageRatingOfAllFeedbacksOfSeller(int sellerId)
        {
            var avgRating = await db.Feedbacks.AsNoTracking().Where(x => x.SellerId == sellerId).AverageAsync(f => (decimal?) f.Rating);
            return avgRating ?? 0;
        }
        public Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Feedback> GetFeedbackByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
