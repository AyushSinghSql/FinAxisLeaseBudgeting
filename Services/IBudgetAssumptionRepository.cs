using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    public interface IBudgetAssumptionRepository
    {
        Task<BudgetAssumptionModel> GetAsync(
                string entityId,
                string propertyId,
                string buildingId,
                string unitId,
                string leaseId);
    }
}
