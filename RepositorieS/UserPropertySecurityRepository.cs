using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class UserPropertySecurityRepository
        : IUserPropertySecurityRepository
    {
        private readonly FinAxisDbContext _context;

        public UserPropertySecurityRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserPropertySecurity>> GetByUserAsync(long userId)
        {
            return await _context.UserPropertySecurities
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.PropertyId)
                .ToListAsync();
        }

        public async Task UpdateUserPropertiesAsync(UserPropertySecurityRequest request)
        {
            using var tran = await _context.Database.BeginTransactionAsync();

            var existing = await _context.UserPropertySecurities
                .Where(x => x.UserId == request.UserId)
                .ToListAsync();

            _context.UserPropertySecurities.RemoveRange(existing);

            foreach (var property in request.Properties)
            {
                _context.UserPropertySecurities.Add(
                    new UserPropertySecurity
                    {
                        UserId = request.UserId,
                        EntityId = property.EntityId,
                        PropertyId = property.PropertyId,
                        AccessLevel = property.AccessLevel,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
            }

            await _context.SaveChangesAsync();

            await tran.CommitAsync();
        }
    }
}
