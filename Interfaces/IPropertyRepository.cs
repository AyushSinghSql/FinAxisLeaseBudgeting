using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IPropertyRepository
    {
        Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10);
        Task<PagedResponse<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10);
    }
}