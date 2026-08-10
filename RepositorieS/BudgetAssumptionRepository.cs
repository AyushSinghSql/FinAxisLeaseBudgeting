//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using FinAxisLeaseBudgeting.Data;
//using FinAxisLeaseBudgeting.Interfaces;
//using FinAxisLeaseBudgeting.Models;
//using Microsoft.EntityFrameworkCore;

//namespace FinAxisLeaseBudgeting.Repositories
//{
//    public class BudgetAssumptionRepository : IBudgetAssumptionRepository
//    {
//        private readonly FinAxisDbContext _context;

//        public BudgetAssumptionRepository(FinAxisDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<BudgetAssumptionModel> GetAsync(
//            string? entityId,
//            string? propertyId,
//            string? buildingId,
//            string? unitId,
//            string? leaseId)
//        {
//            var model = new BudgetAssumptionModel();

//            // 1. Global Level (All scope IDs are null)
//            await ApplyLevel(model,
//                await GetAssumptionDetailsAsync(x =>
//                    x.EntityId == null &&
//                    x.PropertyId == null &&
//                    x.BuildingId == null &&
//                    x.UnitId == null &&
//                    x.LeaseId == null));

//            // 2. Entity Level
//            if (!string.IsNullOrWhiteSpace(entityId))
//            {
//                await ApplyLevel(model,
//                    await GetAssumptionDetailsAsync(x =>
//                        x.EntityId == entityId &&
//                        x.PropertyId == null &&
//                        x.BuildingId == null &&
//                        x.UnitId == null &&
//                        x.LeaseId == null));
//            }

//            // 3. Property Level
//            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId))
//            {
//                await ApplyLevel(model,
//                    await GetAssumptionDetailsAsync(x =>
//                        x.EntityId == entityId &&
//                        x.PropertyId == propertyId &&
//                        x.BuildingId == null &&
//                        x.UnitId == null &&
//                        x.LeaseId == null));
//            }

//            // 4. Building Level
//            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId) && !string.IsNullOrWhiteSpace(buildingId))
//            {
//                await ApplyLevel(model,
//                    await GetAssumptionDetailsAsync(x =>
//                        x.EntityId == entityId &&
//                        x.PropertyId == propertyId &&
//                        x.BuildingId == buildingId &&
//                        x.UnitId == null &&
//                        x.LeaseId == null));
//            }

//            // 5. Unit Level
//            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId) && !string.IsNullOrWhiteSpace(buildingId) && !string.IsNullOrWhiteSpace(unitId))
//            {
//                await ApplyLevel(model,
//                    await GetAssumptionDetailsAsync(x =>
//                        x.EntityId == entityId &&
//                        x.PropertyId == propertyId &&
//                        x.BuildingId == buildingId &&
//                        x.UnitId == unitId &&
//                        x.LeaseId == null));
//            }

//            // 6. Lease Level
//            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId) && !string.IsNullOrWhiteSpace(buildingId) && !string.IsNullOrWhiteSpace(unitId) && !string.IsNullOrWhiteSpace(leaseId))
//            {
//                await ApplyLevel(model,
//                    await GetAssumptionDetailsAsync(x =>
//                        x.EntityId == entityId &&
//                        x.PropertyId == propertyId &&
//                        x.BuildingId == buildingId &&
//                        x.UnitId == unitId &&
//                        x.LeaseId == leaseId));
//            }

//            return model;
//        }

//        public async Task<PlBudgetAssumption?> GetByExactScopeAsync(
//            string? entityId,
//            string? propertyId,
//            string? buildingId,
//            string? unitId,
//            string? leaseId)
//        {
//            return await _context.PlBudgetAssumptions
//                .Include(x => x.AssumptionDetails)
//                .FirstOrDefaultAsync(x =>
//                    x.EntityId == entityId &&
//                    x.PropertyId == propertyId &&
//                    x.BuildingId == buildingId &&
//                    x.UnitId == unitId &&
//                    x.LeaseId == leaseId);
//        }

//        public async Task<PlBudgetAssumption?> GetByIdAsync(long assumptionId)
//        {
//            return await _context.PlBudgetAssumptions
//                .Include(x => x.AssumptionDetails)
//                .FirstOrDefaultAsync(x => x.AssumptionId == assumptionId);
//        }

//        public async Task AddAsync(PlBudgetAssumption assumption)
//        {
//            await _context.PlBudgetAssumptions.AddAsync(assumption);
//        }

//        public Task UpdateAsync(PlBudgetAssumption assumption)
//        {
//            _context.PlBudgetAssumptions.Update(assumption);
//            return Task.CompletedTask;
//        }

//        public async Task SaveChangesAsync()
//        {
//            await _context.SaveChangesAsync();
//        }

//        private async Task<List<PlBudgetAssumptionDetail>> GetAssumptionDetailsAsync(
//            System.Linq.Expressions.Expression<Func<PlBudgetAssumption, bool>> predicate)
//        {
//            return await _context.PlBudgetAssumptions
//                .Where(predicate)
//                .Include(x => x.AssumptionDetails)
//                .SelectMany(x => x.AssumptionDetails)
//                .OrderBy(x => x.SortOrder)
//                .ToListAsync();
//        }

//        private Task ApplyLevel(
//            BudgetAssumptionModel model,
//            List<PlBudgetAssumptionDetail> details)
//        {
//            foreach (var item in details)
//            {
//                switch (item.AssumptionType?.ToUpper())
//                {
//                    case "BASE_RENT_ESCALATION":
//                        model.BaseRentEscalation = item.AssumptionValue ?? 0;
//                        break;

//                    case "CAM_GROWTH":
//                        model.CamGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "TAX_GROWTH":
//                        model.TaxGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "INSURANCE_GROWTH":
//                        model.InsuranceGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "PARKING_GROWTH":
//                        model.ParkingGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "STORAGE_GROWTH":
//                        model.StorageGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "BAD_DEBT":
//                        model.BadDebt = item.AssumptionValue ?? 0;
//                        break;

//                    case "VACANCY":
//                        model.Vacancy = item.AssumptionValue ?? 0;
//                        break;

//                    case "MARKET_RENT_GROWTH":
//                        model.MarketRentGrowth = item.AssumptionValue ?? 0;
//                        break;

//                    case "RENEWAL_INCREASE":
//                        model.RenewalIncrease = item.AssumptionValue ?? 0;
//                        break;

//                    case "RENEWAL_PROBABILITY":
//                        model.RenewalProbability = item.AssumptionValue ?? 0;
//                        break;

//                    case "FREE_RENT_MONTHS":
//                        model.FreeRentMonths = Convert.ToInt32(item.AssumptionValue ?? 0);
//                        break;
//                }
//            }

//            return Task.CompletedTask;
//        }
//        public async Task SaveOrUpdateAssumptionsAsync(
//            string? entityId,
//            string? propertyId,
//            string? buildingId,
//            string? unitId,
//            string? leaseId,
//            PlBudgetAssumption payload,
//            string userId)
//        {
//            var existing = await GetByExactScopeAsync(entityId, propertyId, buildingId, unitId, leaseId);
//            var currentDate = DateTime.UtcNow;

//            if (existing == null)
//            {
//                payload.EntityId = entityId;
//                payload.PropertyId = propertyId;
//                payload.BuildingId = buildingId;
//                payload.UnitId = unitId;
//                payload.LeaseId = leaseId;
//                payload.CreatedBy = userId;
//                payload.CreatedOn = currentDate;

//                foreach (var detail in payload.AssumptionDetails)
//                {
//                    detail.CreatedBy = userId;
//                    detail.CreatedOn = currentDate;
//                }

//                await AddAsync(payload);
//            }
//            else
//            {
//                existing.Remarks = payload.Remarks;
//                existing.AssumptionName = payload.AssumptionName;
//                existing.UpdatedBy = userId;
//                existing.UpdatedOn = currentDate;

//                existing.AssumptionDetails.Clear();
//                foreach (var detail in payload.AssumptionDetails)
//                {
//                    detail.AssumptionId = existing.AssumptionId;
//                    detail.UpdatedBy = userId;
//                    detail.UpdatedOn = currentDate;
//                    existing.AssumptionDetails.Add(detail);
//                }

//                await UpdateAsync(existing);
//            }

//            await SaveChangesAsync();
//        }

//    }
//}


using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Repositories
{
    public class BudgetAssumptionRepository : IBudgetAssumptionRepository
    {
        private readonly FinAxisDbContext _context;

        public BudgetAssumptionRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetAssumptionModel> GetAsync(string? entityId, string? propertyId, string? unitId, string? leaseId)
        {
            var model = new BudgetAssumptionModel();

            // 1. Global Level
            await ApplyLevel(model, await GetAssumptionDetailsAsync(x => x.EntityId == null && x.PropertyId == null && x.UnitId == null && x.LeaseId == null));

            // 2. Entity Level
            if (!string.IsNullOrWhiteSpace(entityId))
                await ApplyLevel(model, await GetAssumptionDetailsAsync(x => x.EntityId == entityId && x.PropertyId == null && x.UnitId == null && x.LeaseId == null));

            // 3. Property Level
            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId))
                await ApplyLevel(model, await GetAssumptionDetailsAsync(x => x.EntityId == entityId && x.PropertyId == propertyId && x.UnitId == null && x.LeaseId == null));

            // 4. Unit Level
            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId) && !string.IsNullOrWhiteSpace(unitId))
                await ApplyLevel(model, await GetAssumptionDetailsAsync(x => x.EntityId == entityId && x.PropertyId == propertyId && x.UnitId == unitId && x.LeaseId == null));

            // 5. Lease Level
            if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(propertyId) && !string.IsNullOrWhiteSpace(unitId) && !string.IsNullOrWhiteSpace(leaseId))
                await ApplyLevel(model, await GetAssumptionDetailsAsync(x => x.EntityId == entityId && x.PropertyId == propertyId && x.UnitId == unitId && x.LeaseId == leaseId));

            return model;
        }

        public async Task<List<PlBudgetAssumption>> GetByExactScopeAsync(string? entityId, string? propertyId, string? unitId, string? leaseId)
        {
            return await _context.PlBudgetAssumptions
                .AsNoTracking()
                .Include(x => x.AssumptionDetails)
                .Where(x => x.EntityId == entityId && x.PropertyId == propertyId && x.UnitId == unitId && x.LeaseId == leaseId)
                .ToListAsync();
        }

        public async Task<PlBudgetAssumption?> GetByIdAsync(long assumptionId)
        {
            return await _context.PlBudgetAssumptions
                .Include(x => x.AssumptionDetails)
                .FirstOrDefaultAsync(x => x.AssumptionId == assumptionId);
        }

        public async Task AddAsync(PlBudgetAssumption assumption) => await _context.PlBudgetAssumptions.AddAsync(assumption);

        public Task UpdateAsync(PlBudgetAssumption assumption)
        {
            _context.PlBudgetAssumptions.Update(assumption);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        private async Task<List<PlBudgetAssumptionDetail>> GetAssumptionDetailsAsync(System.Linq.Expressions.Expression<Func<PlBudgetAssumption, bool>> predicate)
        {
            return await _context.PlBudgetAssumptions
                .Where(predicate)
                .Include(x => x.AssumptionDetails)
                .SelectMany(x => x.AssumptionDetails)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        private Task ApplyLevel(BudgetAssumptionModel model, List<PlBudgetAssumptionDetail> details)
        {
            foreach (var item in details)
            {
                switch (item.AssumptionType?.ToUpper())
                {
                    case "BASE_RENT_ESCALATION": model.BaseRentEscalation = item.AssumptionValue ?? 0; break;
                    case "CAM_GROWTH": model.CamGrowth = item.AssumptionValue ?? 0; break;
                    case "TAX_GROWTH": model.TaxGrowth = item.AssumptionValue ?? 0; break;
                    case "INSURANCE_GROWTH": model.InsuranceGrowth = item.AssumptionValue ?? 0; break;
                    case "PARKING_GROWTH": model.ParkingGrowth = item.AssumptionValue ?? 0; break;
                    case "STORAGE_GROWTH": model.StorageGrowth = item.AssumptionValue ?? 0; break;
                    case "BAD_DEBT": model.BadDebt = item.AssumptionValue ?? 0; break;
                    case "VACANCY": model.Vacancy = item.AssumptionValue ?? 0; break;
                    case "MARKET_RENT_GROWTH": model.MarketRentGrowth = item.AssumptionValue ?? 0; break;
                    case "RENEWAL_INCREASE": model.RenewalIncrease = item.AssumptionValue ?? 0; break;
                    case "RENEWAL_PROBABILITY": model.RenewalProbability = item.AssumptionValue ?? 0; break;
                    case "FREE_RENT_MONTHS": model.FreeRentMonths = Convert.ToInt32(item.AssumptionValue ?? 0); break;
                }
            }
            return Task.CompletedTask;
        }

        public async Task SaveOrUpdateAssumptionsAsync(string? entityId, string? propertyId, string? unitId, string? leaseId, PlBudgetAssumption payload, string userId)
        {
            var existing = await GetByExactScopeAsync(entityId, propertyId, unitId, leaseId);
            var currentDate = DateTime.UtcNow;

            if (existing == null)
            {
                payload.EntityId = entityId;
                payload.PropertyId = propertyId;
                payload.UnitId = unitId;
                payload.LeaseId = leaseId;
                payload.CreatedBy = userId;
                payload.CreatedOn = currentDate;

                foreach (var detail in payload.AssumptionDetails)
                {
                    detail.CreatedBy = userId;
                    detail.CreatedOn = currentDate;
                }

                await AddAsync(payload);
            }
            else
            {
                existing.Remarks = payload.Remarks;
                existing.AssumptionName = payload.AssumptionName;
                existing.UpdatedBy = userId;
                existing.UpdatedOn = currentDate;

                existing.AssumptionDetails.Clear();
                foreach (var detail in payload.AssumptionDetails)
                {
                    detail.AssumptionId = existing.AssumptionId;
                    detail.UpdatedBy = userId;
                    detail.UpdatedOn = currentDate;
                    existing.AssumptionDetails.Add(detail);
                }

                await UpdateAsync(existing);
            }

            await SaveChangesAsync();
        }
    }

    public class BudgetLookupRepository : IBudgetLookupRepository
    {
        private readonly FinAxisDbContext _context;

        public BudgetLookupRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LookupItemDto>> GetEntitiesAsync()
        {
            return await _context.PlBudgetAssumptions
                .Where(x => x.EntityId != null)
                .Select(x => new LookupItemDto { Id = x.EntityId!, Name = x.EntityId! })
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<LookupItemDto>> GetPropertiesAsync(string? entityId)
        {
            return await _context.PlBudgetAssumptions
                .Where(x => (entityId == null || x.EntityId == entityId) && x.PropertyId != null)
                .Select(x => new LookupItemDto { Id = x.PropertyId!, Name = x.PropertyId! })
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<LookupItemDto>> GetUnitsAsync(string? propertyId)
        {
            return await _context.PlBudgetAssumptions
                .Where(x => (propertyId == null || x.PropertyId == propertyId) && x.UnitId != null)
                .Select(x => new LookupItemDto { Id = x.UnitId!, Name = x.UnitId! })
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<LookupItemDto>> GetLeasesAsync(string? unitId)
        {
            return await _context.PlBudgetAssumptions
                .Where(x => (unitId == null || x.UnitId == unitId) && x.LeaseId != null)
                .Select(x => new LookupItemDto { Id = x.LeaseId!, Name = x.LeaseId! })
                .Distinct()
                .ToListAsync();
        }
    }
}