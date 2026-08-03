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
        Task<List<PlLeaseBudget>> GetBudgetsAsync(LeaseBudgetSearchRequest request);

        Task BulkUpdateRevenueAsync(BulkUpdateLeaseRevenueRequest request);

        Task<LeaseBudgetDto?> GetBudgetByIdAsync(long budgetId);

         //RevenueCalculationResult Calculate(
         //       LeaseMaster lease,
         //       //PlLeaseRentSchedule rentSchedule,
         //       BudgetAssumptionModel assumptions,
         //       DateOnly budgetMonth);
    }
}
