using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class CommLeaseRepository : ICommLeaseRepository
    {
        private readonly FinAxisDbContext _context;

        public CommLeaseRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommLease>> GetAllAsync()
        {
            return await _context.CommLeases.ToListAsync();
        }

        public async Task<CommLease?> GetByIdAsync(long id)
        {
            return await _context.CommLeases.FindAsync(id);
        }

        public async Task<CommLease?> GetByLeaseCodeAsync(string leaseCode)
        {
            return await _context.CommLeases.FirstOrDefaultAsync(l => l.LeaseCode == leaseCode);
        }

        public async Task AddAsync(CommLease lease)
        {
            await _context.CommLeases.AddAsync(lease);
        }

        public void Update(CommLease lease)
        {
            _context.CommLeases.Update(lease);
        }

        public void Delete(CommLease lease)
        {
            _context.CommLeases.Remove(lease);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
