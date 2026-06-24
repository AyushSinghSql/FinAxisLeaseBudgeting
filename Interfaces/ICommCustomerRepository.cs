using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ICommCustomerRepository
    {
        Task<IEnumerable<CommCustomer>> GetAllAsync();
        Task<CommCustomer?> GetByIdAsync(long id);
        Task<CommCustomer?> GetByCustomerCodeAsync(string customerCode);
        Task AddAsync(CommCustomer customer);
        void Update(CommCustomer customer);
        void Delete(CommCustomer customer);
        Task<bool> SaveChangesAsync();
    }
}
