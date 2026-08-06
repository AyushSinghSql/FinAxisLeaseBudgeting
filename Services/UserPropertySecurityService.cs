using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    public class UserPropertySecurityService
    {
        private readonly IUserPropertySecurityRepository _repository;

        public UserPropertySecurityService(
            IUserPropertySecurityRepository repository)
        {
            _repository = repository;
        }

        public async Task UpdateUserPropertiesAsync(
            UserPropertySecurityRequest request)
        {
            await _repository.UpdateUserPropertiesAsync(request);
        }

        public async Task<List<UserPropertySecurity>> GetByUserAsync(long userId)
        {
            return await _repository.GetByUserAsync(userId);
        }
    }
}
