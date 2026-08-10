using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IBudgetAssumptionRepository
    {
        Task<BudgetAssumptionModel> GetAsync(string? entityId, string? propertyId, string? unitId, string? leaseId);
        Task<IEnumerable<PlBudgetAssumption>> GetByExactScopeAsync(string? assumptionId, string? entityId, string? propertyId, string? unitId, string? leaseId);
        Task<BudgetAssumptionModel?> GetByIdAsync(long assumptionId);
        Task AddAsync(PlBudgetAssumption assumption);
        Task UpdateAsync(PlBudgetAssumption assumption);
        Task SaveChangesAsync();
        Task SaveOrUpdateAssumptionsAsync(string? assumptionId, string? entityId, string? propertyId, string? unitId, string? leaseId, PlBudgetAssumption payload, string userId);
    }

    public interface IBudgetLookupRepository
    {
        Task<IEnumerable<LookupItemDto>> GetEntitiesAsync();
        Task<IEnumerable<LookupItemDto>> GetPropertiesAsync(string? entityId);
        Task<IEnumerable<LookupItemDto>> GetUnitsAsync(string? propertyId);
        Task<IEnumerable<LookupItemDto>> GetLeasesAsync(string? unitId);

        Task<IEnumerable<LookupItemDto>> GetAssumptionsAsync(long? assumptionId);
    }

}