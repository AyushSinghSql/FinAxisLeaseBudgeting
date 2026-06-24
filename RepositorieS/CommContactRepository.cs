using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class CommContactRepository : ICommContactRepository
    {
        private readonly FinAxisDbContext _context;

        public CommContactRepository(FinAxisDbContext context) => _context = context;

        public async Task<IEnumerable<CommContact>> GetAllAsync() => await _context.CommContacts.ToListAsync();

        public async Task<CommContact?> GetByIdAsync(long id) => await _context.CommContacts.FindAsync(id);

        public async Task AddAsync(CommContact contact) => await _context.CommContacts.AddAsync(contact);

        public void Update(CommContact contact) => _context.CommContacts.Update(contact);

        public void Delete(CommContact contact) => _context.CommContacts.Remove(contact);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}
