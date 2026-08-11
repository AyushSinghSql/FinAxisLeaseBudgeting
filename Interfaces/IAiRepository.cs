using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IAiRepository
    {
        Task<PagedResponse<ExpiringLeaseDto>> GetExpiringLeasesAsync(
            string? propertyId = null,
            int value = 1,
            string timeUnit = "month",
            int pageNumber = 0,
            int pageSize = 10);

        Task<PagedResponse<VacantUnitDto>> GetVacantUnitsAsync(
            string? propertyId = null,
            int pageNumber = 0,
            int pageSize = 10);

        Task<PagedResponse<MarketRentDto>> GetMarketRentUnitsAsync(
            string? propertyId = null,
            decimal? minRent = null,
            decimal? maxRent = null,
            string? unitType = null,
            string? unitStatus = null,
            decimal? minArea = null,
            decimal? maxArea = null,
            int pageNumber = 0,
            int pageSize = 10);

        Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            string? entityId = null,
            string? propertyId = null,
            string? buildingId = null,
            string? unitId = null,
            string? leaseId = null,
            string? tenantId = null,
            int pageNumber = 0,
            int pageSize = 10);

        // Retained for backward compatibility
        Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            string? entityId,
            string? propertyId,
            string? buildingId,
            string? unitId,
            string? leaseId,
            int pageNumber,
            int pageSize);

        Task<PagedResponse<object>> GetMasterDataAsync(
            string masterType,
            string? searchFilter = null,
            int pageNumber = 0,
            int pageSize = 10);
    }
}