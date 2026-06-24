using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ICommLeaseUnitRepository
    {
        Task<IEnumerable<CommLeaseUnit>> GetAllAsync();
        Task<CommLeaseUnit?> GetByIdAsync(long id);
        Task<IEnumerable<CommLeaseUnit>> GetByLeaseCodeAsync(string leaseCode);
        Task AddAsync(CommLeaseUnit leaseUnit);
        void Update(CommLeaseUnit leaseUnit);
        void Delete(CommLeaseUnit leaseUnit);
        Task<bool> SaveChangesAsync();
    }
}
