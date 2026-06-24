using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class CommLeaseUnitRepository : ICommLeaseUnitRepository
    {
        private readonly FinAxisDbContext _context;

        public CommLeaseUnitRepository(FinAxisDbContext context) => _context = context;

        public async Task<IEnumerable<CommLeaseUnit>> GetAllAsync() => await _context.CommLeaseUnits.ToListAsync();

        public async Task<CommLeaseUnit?> GetByIdAsync(long id) => await _context.CommLeaseUnits.FindAsync(id);

        public async Task<IEnumerable<CommLeaseUnit>> GetByLeaseCodeAsync(string leaseCode) =>
            await _context.CommLeaseUnits.Where(lu => lu.LeaseCode == leaseCode).ToListAsync();

        public async Task AddAsync(CommLeaseUnit leaseUnit) => await _context.CommLeaseUnits.AddAsync(leaseUnit);

        public void Update(CommLeaseUnit leaseUnit) => _context.CommLeaseUnits.Update(leaseUnit);

        public void Delete(CommLeaseUnit leaseUnit) => _context.CommLeaseUnits.Remove(leaseUnit);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}
