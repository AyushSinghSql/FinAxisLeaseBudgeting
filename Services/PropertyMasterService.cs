using System.Collections.Generic;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Repositories;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    public interface IPropertyService
    {
        Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
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
    }
}