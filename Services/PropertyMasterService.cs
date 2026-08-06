using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinAxisLeaseBudgeting.Services
{
    public interface IPropertyService
    {
        Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm);
        Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownByUserAsync(int userId, string? searchTerm = null);
        Task<PagedResponse<PropertyBudgetDetailDto>> GetPropertyBudgetDetailsAsync(string? searchTerm, int pageNumber, int pageSize);
    }

    public class PropertyMasterService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyMasterService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            return await _propertyRepository.GetPropertiesAsync(searchTerm, pageNumber, pageSize);
        }

        public async Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm)
        {
            return await _propertyRepository.GetPropertyDropdownAsync(searchTerm);
        }

        public async Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownByUserAsync(int userId, string? searchTerm = null)
        {
            return await _propertyRepository.GetPropertyDropdownByUserAsync(userId, searchTerm);
        }

        public async Task<PagedResponse<PropertyBudgetDetailDto>> GetPropertyBudgetDetailsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            return await _propertyRepository.GetPropertyBudgetDetailsAsync(searchTerm, pageNumber, pageSize);
        }
    }
}