using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IUserPropertySecurityRepository
    {
        Task UpdateUserPropertiesAsync(UserPropertySecurityRequest request);

        Task<List<UserPropertySecurity>> GetByUserAsync(long userId);
    }
}
