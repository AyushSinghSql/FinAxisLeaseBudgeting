using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    // Interface definition
    public interface IPropertyService
    {
        Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<PagedResponse<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm, int pageNumber, int pageSize);
    }

    // Concrete class implementation
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

        public async Task<PagedResponse<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            return await _propertyRepository.GetPropertyDropdownAsync(searchTerm, pageNumber, pageSize);
        }
    }
}