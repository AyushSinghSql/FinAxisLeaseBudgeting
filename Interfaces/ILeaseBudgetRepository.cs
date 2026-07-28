using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ILeaseBudgetRepository
    {
        Task<LeaseBudgetResponse> GenerateRevenueBudgetAsync(
            GenerateLeaseBudgetRequest request);

        Task<List<PlLeaseBudget>> SearchAsync(LeaseBudgetSearchRequest request);

        Task BulkUpdateRevenueAsync(BulkUpdateLeaseRevenueRequest request);

         //RevenueCalculationResult Calculate(
         //       LeaseMaster lease,
         //       //PlLeaseRentSchedule rentSchedule,
         //       BudgetAssumptionModel assumptions,
         //       DateOnly budgetMonth);
    }
}
