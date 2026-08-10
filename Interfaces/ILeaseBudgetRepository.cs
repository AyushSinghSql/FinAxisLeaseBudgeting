using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ILeaseBudgetRepository
    {
        Task<LeaseBudgetResponse> GenerateRevenueBudgetAsync(
            GenerateLeaseBudgetRequest request);

        Task<LeaseBudgetResponse> GenerateRevenueBudgetAsyncV1(
            GenerateLeaseBudgetRequest request);

        Task<long> SaveLeaseBudgetAsyncV1(
LeaseBudgetResponse response,
string propertyId,
string unitId,
string leaseId,
int version,
DateOnly? startDate,
DateOnly? endDate,
string budgetType,
string generatedBy);

        Task<List<PlLeaseBudget>> SearchAsync(LeaseBudgetSearchRequest request);
        Task<List<LeaseBudgetDto>> SearchAsyncV1(LeaseBudgetSearchRequest request);
        Task<List<LeaseBudgetChargeGroupDto>> SearchAsyncV2();
        Task<List<PlLeaseBudget>> GetBudgetsAsync(LeaseBudgetSearchRequest request);

        Task BulkUpdateRevenueAsync(BulkUpdateLeaseRevenueRequest request);

        Task BulkUpsertAsync(List<PlLeaseBudgetDetail> details);

        Task BulkDeleteAsync(int BudgetId, List<string> chargeCodes);

        Task<LeaseBudgetDto?> GetBudgetByIdAsync(long budgetId);

        Task<bool> UpdateProperityBudgetAsync(PlLeaseBudget budget);

         //RevenueCalculationResult Calculate(
         //       LeaseMaster lease,
         //       //PlLeaseRentSchedule rentSchedule,
         //       BudgetAssumptionModel assumptions,
         //       DateOnly budgetMonth);
    }
}
