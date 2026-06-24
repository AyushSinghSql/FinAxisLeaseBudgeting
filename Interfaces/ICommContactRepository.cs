using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ICommContactRepository
    {
        Task<IEnumerable<CommContact>> GetAllAsync();
        Task<CommContact?> GetByIdAsync(long id);
        Task AddAsync(CommContact contact);
        void Update(CommContact contact);
        void Delete(CommContact contact);
        Task<bool> SaveChangesAsync();
    }
}
