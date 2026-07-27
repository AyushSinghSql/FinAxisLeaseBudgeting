using System.Collections.Generic;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ILeaseChargeRepository
    {
        Task<PagedResponse<LeaseCharge>> GetLeaseChargesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10);
        Task<IEnumerable<LeaseChargeDropdownDto>> GetLeaseChargeDropdownAsync(string? searchTerm = null);
    }
}