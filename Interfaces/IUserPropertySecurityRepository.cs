using FinAxisLeaseBudgeting.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IUserPropertySecurityRepository
    {
        Task UpdateUserPropertiesAsync(UserPropertySecurityRequest request);

        Task<List<UserPropertySecurity>> GetByUserAsync(long userId);

        Task<bool> ValidateUserAccessAsync(long userId, string entityId, string propertyId = null, string unitId = null);

        Task<string> GetUserAllowedScopeSummaryAsync(long userId);
    }
}