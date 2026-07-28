using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    using FinAxisLeaseBudgeting.Data;
    using FinAxisLeaseBudgeting.Services;
    using Microsoft.EntityFrameworkCore;

    public class BudgetAssumptionRepository : IBudgetAssumptionRepository
    {
        private readonly FinAxisDbContext _context;

        public BudgetAssumptionRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetAssumptionModel> GetAsync(
            string? entityId,
            string? propertyId,
            string? buildingId,
            string? unitId,
            string? leaseId)
        {
            var model = new BudgetAssumptionModel();

            // Global
            await ApplyLevel(model,
                await GetAssumptionDetailsAsync(x =>
                    x.EntityId == null &&
                    x.PropertyId == null &&
                    x.BuildingId == null &&
                    x.UnitId == null &&
                    x.LeaseId == null));

            // Entity
            if (!string.IsNullOrWhiteSpace(entityId))
            {
                await ApplyLevel(model,
                    await GetAssumptionDetailsAsync(x =>
                        x.EntityId == entityId &&
                        x.PropertyId == null &&
                        x.BuildingId == null &&
                        x.UnitId == null &&
                        x.LeaseId == null));
            }

            // Property
            if (!string.IsNullOrWhiteSpace(propertyId))
            {
                await ApplyLevel(model,
                    await GetAssumptionDetailsAsync(x =>
                        x.EntityId == entityId &&
                        x.PropertyId == propertyId &&
                        x.BuildingId == null &&
                        x.UnitId == null &&
                        x.LeaseId == null));
            }

            // Building
            if (!string.IsNullOrWhiteSpace(buildingId))
            {
                await ApplyLevel(model,
                    await GetAssumptionDetailsAsync(x =>
                        x.EntityId == entityId &&
                        x.PropertyId == propertyId &&
                        x.BuildingId == buildingId &&
                        x.UnitId == null &&
                        x.LeaseId == null));
            }

            // Unit
            if (!string.IsNullOrWhiteSpace(unitId))
            {
                await ApplyLevel(model,
                    await GetAssumptionDetailsAsync(x =>
                        x.EntityId == entityId &&
                        x.PropertyId == propertyId &&
                        x.BuildingId == buildingId &&
                        x.UnitId == unitId &&
                        x.LeaseId == null));
            }

            // Lease
            if (!string.IsNullOrWhiteSpace(leaseId))
            {
                await ApplyLevel(model,
                    await GetAssumptionDetailsAsync(x =>
                        x.EntityId == entityId &&
                        x.PropertyId == propertyId &&
                        x.BuildingId == buildingId &&
                        x.UnitId == unitId &&
                        x.LeaseId == leaseId));
            }

            return model;
        }

        private async Task<List<PlBudgetAssumptionDetail>> GetAssumptionDetailsAsync(
            System.Linq.Expressions.Expression<Func<PlBudgetAssumption, bool>> predicate)
        {
            return await _context.PlBudgetAssumptions
                .Where(predicate)
                .Include(x => x.AssumptionDetails)
                .SelectMany(x => x.AssumptionDetails)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        private Task ApplyLevel(
            BudgetAssumptionModel model,
            List<PlBudgetAssumptionDetail> details)
        {
            foreach (var item in details)
            {
                switch (item.AssumptionType)
                {
                    case "BASE_RENT_ESCALATION":
                        model.BaseRentEscalation = item.AssumptionValue ?? 0;
                        break;

                    case "CAM_GROWTH":
                        model.CamGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "TAX_GROWTH":
                        model.TaxGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "INSURANCE_GROWTH":
                        model.InsuranceGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "PARKING_GROWTH":
                        model.ParkingGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "STORAGE_GROWTH":
                        model.StorageGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "BAD_DEBT":
                        model.BadDebt = item.AssumptionValue ?? 0;
                        break;

                    case "VACANCY":
                        model.Vacancy = item.AssumptionValue ?? 0;
                        break;

                    case "MARKET_RENT_GROWTH":
                        model.MarketRentGrowth = item.AssumptionValue ?? 0;
                        break;

                    case "RENEWAL_INCREASE":
                        model.RenewalIncrease = item.AssumptionValue ?? 0;
                        break;

                    case "RENEWAL_PROBABILITY":
                        model.RenewalProbability = item.AssumptionValue ?? 0;
                        break;

                    case "FREE_RENT_MONTHS":
                        model.FreeRentMonths = Convert.ToInt32(item.AssumptionValue ?? 0);
                        break;
                }
            }

            return Task.CompletedTask;
        }

    }
}
