using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IUnitRepository
    {
        Task<PagedResponse<UnitMaster>> GetUnitsAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10);
    }
}
