using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ICommLeaseRepository
    {
        Task<IEnumerable<CommLease>> GetAllAsync();
        Task<CommLease?> GetByIdAsync(long id);
        Task<CommLease?> GetByLeaseCodeAsync(string leaseCode);
        Task AddAsync(CommLease lease);
        void Update(CommLease lease);
        void Delete(CommLease lease);
        Task<bool> SaveChangesAsync();
    }
}
