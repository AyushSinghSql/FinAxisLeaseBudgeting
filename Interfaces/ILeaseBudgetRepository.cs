using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface ILeaseBudgetRepository
    {
        Task<LeaseBudgetResponse> GenerateRevenueBudgetAsync(
            GenerateLeaseBudgetRequest request);

        Task<List<PlLeaseBudget>> SearchAsync(LeaseBudgetSearchRequest request);
    }
}
