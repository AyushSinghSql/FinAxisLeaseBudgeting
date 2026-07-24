using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ILeaseRepository
    {
        Task<PagedResponse<LeaseMaster>> GetLeasesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10);
    }
}