using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Feedbacks
{
    public interface IFeedbackRepository
    {
        Task<Feedback> GetFeedbackByIdAsync(int id);
        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback> AddFeedbackAsync(Feedback feedback);
        Task<decimal> GetAverageRatingOfAllFeedbacksOfSeller(int sellerId);
        Task<IEnumerable<Feedback>> GetAllFeedbacksOfSeller(int sellerId);
    }
}
