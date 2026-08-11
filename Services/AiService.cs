using System.Collections.Generic;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    public interface IAiService
    {
        // Updated to support flexible time units (days, weeks, months, years)
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
            int pageNumber = 0,
            int pageSize = 10);

        Task<PagedResponse<object>> GetMasterDataAsync(
            string masterType,
            string? searchFilter = null,
            int pageNumber = 0,
            int pageSize = 10);
    }

    public class AiService : IAiService
    {
        private readonly IAiRepository _aiRepository;

        public AiService(IAiRepository aiRepository)
        {
            _aiRepository = aiRepository;
        }

        // Updated implementation matching the interface signature
        public async Task<PagedResponse<ExpiringLeaseDto>> GetExpiringLeasesAsync(
            string? propertyId = null,
            int value = 1,
            string timeUnit = "month",
            int pageNumber = 0,
            int pageSize = 10)
        {
            return await _aiRepository.GetExpiringLeasesAsync(propertyId, value, timeUnit, pageNumber, pageSize);
        }

        public async Task<PagedResponse<VacantUnitDto>> GetVacantUnitsAsync(string? propertyId = null, int pageNumber = 0, int pageSize = 10)
        {
            return await _aiRepository.GetVacantUnitsAsync(propertyId, pageNumber, pageSize);
        }

        public async Task<PagedResponse<MarketRentDto>> GetMarketRentUnitsAsync(
            string? propertyId = null,
            decimal? minRent = null,
            decimal? maxRent = null,
            string? unitType = null,
            string? unitStatus = null,
            decimal? minArea = null,
            decimal? maxArea = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            return await _aiRepository.GetMarketRentUnitsAsync(
                propertyId, minRent, maxRent, unitType, unitStatus, minArea, maxArea, pageNumber, pageSize);
        }

        public async Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            string? entityId = null,
            string? propertyId = null,
            string? buildingId = null,
            string? unitId = null,
            string? leaseId = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            return await _aiRepository.GetBudgetAssumptionsAsync(
                entityId, propertyId, buildingId, unitId, leaseId, pageNumber, pageSize);
        }

        public async Task<PagedResponse<object>> GetMasterDataAsync(
            string masterType,
            string? searchFilter = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            return await _aiRepository.GetMasterDataAsync(masterType, searchFilter, pageNumber, pageSize);
        }
    }
}