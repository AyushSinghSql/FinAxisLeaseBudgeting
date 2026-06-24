using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class CommCustomerRepository : ICommCustomerRepository
    {
        private readonly FinAxisDbContext _context;

        public CommCustomerRepository(FinAxisDbContext context) => _context = context;

        public async Task<IEnumerable<CommCustomer>> GetAllAsync() => await _context.CommCustomers.ToListAsync();

        public async Task<CommCustomer?> GetByIdAsync(long id) => await _context.CommCustomers.FindAsync(id);

        public async Task<CommCustomer?> GetByCustomerCodeAsync(string customerCode)
        {
            return await _context.CommCustomers
                .Where(c => c.CustomerCode == customerCode)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(CommCustomer customer) => await _context.CommCustomers.AddAsync(customer);

        public void Update(CommCustomer customer) => _context.CommCustomers.Update(customer);

        public void Delete(CommCustomer customer) => _context.CommCustomers.Remove(customer);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}
