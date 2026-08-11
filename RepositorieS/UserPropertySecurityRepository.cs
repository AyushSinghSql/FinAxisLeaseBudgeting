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


        public async Task<bool> ValidateUserAccessAsync(long userId, string entityId, string propertyId = null, string unitId = null)
        {
            var securities = await GetByUserAsync(userId);
            var activeSecurities = securities.Where(x => x.IsActive).ToList();

            if (!activeSecurities.Any()) return false;

            // Rule 1: If an Entity is assigned with a null/empty PropertyId, ALL properties/units under it are allowed.
            bool broadEntityMatch = activeSecurities.Any(x => x.EntityId == entityId && string.IsNullOrEmpty(x.PropertyId));
            if (broadEntityMatch) return true;

            // Verify if entity is authorized at all
            if (!string.IsNullOrEmpty(entityId))
            {
                var allowedEntities = activeSecurities.Select(x => x.EntityId).Distinct().ToList();
                if (!allowedEntities.Contains(entityId)) return false;
            }

            // Rule 2: Check Property Level
            if (!string.IsNullOrEmpty(propertyId))
            {
                var allowedProperties = activeSecurities
                    .Where(x => !string.IsNullOrEmpty(x.PropertyId))
                    .Select(x => x.PropertyId)
                    .Distinct()
                    .ToList();

                if (allowedProperties.Any() && !allowedProperties.Contains(propertyId))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(unitId))
            {
            }

            return true;
        }

        public async Task<string> GetUserAllowedScopeSummaryAsync(long userId)
        {
            var securities = await GetByUserAsync(userId);
            var activeSecurities = securities.Where(x => x.IsActive).ToList();

            if (!activeSecurities.Any()) return "No assigned entities or properties.";

            var summaryLines = new List<string>();
            var groupedByEntity = activeSecurities.GroupBy(x => x.EntityId);

            foreach (var group in groupedByEntity)
            {
                var props = group.Where(g => !string.IsNullOrEmpty(g.PropertyId)).Select(g => g.PropertyId).ToList();
                if (!props.Any())
                {
                    summaryLines.Add($"Entity: {group.Key} (All Properties & Units)");
                }
                else
                {
                    summaryLines.Add($"Entity: {group.Key}, Assigned Properties: {string.Join(", ", props)}");
                }
            }

            return string.Join("; ", summaryLines);
        }
    }
}
