using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Repositories
{
    public class AiRepository : IAiRepository
    {
        private readonly FinAxisDbContext _context;

        public AiRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<ExpiringLeaseDto>> GetExpiringLeasesAsync(
            string? propertyId = null,
            int months = 1,
            int pageNumber = 0,
            int pageSize = 10)
        {
            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddMonths(months).AddDays(1);

            IQueryable<LeaseMaster> query = _context.LeaseMasters.AsNoTracking()
                .Where(l => l.LeaseEndDate.HasValue
                         && l.LeaseEndDate.Value >= today
                         && l.LeaseEndDate.Value < targetDate);

            if (!string.IsNullOrWhiteSpace(propertyId))
            {
                query = query.Where(l => l.PropertyId == propertyId);
            }

            int totalRecords = await query.CountAsync();

            var projectedQuery = query
                .OrderBy(l => l.LeaseEndDate)
                .Select(l => new ExpiringLeaseDto
                {
                    LeaseId = l.LeaseId,
                    TenantName = l.TenantName ?? string.Empty,
                    PropertyId = l.PropertyId,
                    UnitId = l.UnitId,
                    LeaseEndDate = l.LeaseEndDate,
                    ContractRent = l.ContractRent,
                    LeaseStatus = l.LeaseStatus
                });

            var (data, calculatedPageSize, totalPages) = await ExecutePaginationAsync(projectedQuery, totalRecords, pageNumber, pageSize);

            return new PagedResponse<ExpiringLeaseDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber <= 0 ? 0 : pageNumber,
                PageSize = calculatedPageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResponse<VacantUnitDto>> GetVacantUnitsAsync(
            string? propertyId = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            IQueryable<UnitMaster> query = _context.UnitMasters.AsNoTracking()
                .Where(u => u.UnitStatus != null && EF.Functions.Like(u.UnitStatus, "vacant"));

            if (!string.IsNullOrWhiteSpace(propertyId))
            {
                query = query.Where(u => u.PropertyId == propertyId);
            }

            int totalRecords = await query.CountAsync();

            var projectedQuery = query
                .OrderBy(u => u.UnitCode)
                .Select(u => new VacantUnitDto
                {
                    UnitId = u.UnitId,
                    UnitCode = u.UnitCode,
                    PropertyId = u.PropertyId,
                    UnitType = u.UnitType,
                    UnitStatus = u.UnitStatus,
                    Area = u.Area,
                    Building = u.Building,
                    Floor = u.Floor,
                    MarketRent = u.MarketRent
                });

            var (data, calculatedPageSize, totalPages) = await ExecutePaginationAsync(projectedQuery, totalRecords, pageNumber, pageSize);

            return new PagedResponse<VacantUnitDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber <= 0 ? 0 : pageNumber,
                PageSize = calculatedPageSize,
                TotalPages = totalPages
            };
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
            IQueryable<UnitMaster> query = _context.UnitMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(propertyId))
                query = query.Where(u => u.PropertyId == propertyId);

            if (minRent.HasValue)
                query = query.Where(u => u.MarketRent >= minRent.Value);

            if (maxRent.HasValue)
                query = query.Where(u => u.MarketRent <= maxRent.Value);

            if (!string.IsNullOrWhiteSpace(unitType))
                query = query.Where(u => u.UnitType != null && EF.Functions.Like(u.UnitType, unitType));

            if (!string.IsNullOrWhiteSpace(unitStatus))
                query = query.Where(u => u.UnitStatus != null && EF.Functions.Like(u.UnitStatus, unitStatus));

            if (minArea.HasValue)
                query = query.Where(u => u.Area >= minArea.Value);

            if (maxArea.HasValue)
                query = query.Where(u => u.Area <= maxArea.Value);

            int totalRecords = await query.CountAsync();

            var projectedQuery = query
                .OrderBy(u => u.PropertyId)
                .ThenBy(u => u.UnitCode)
                .Select(u => new MarketRentDto
                {
                    UnitId = u.UnitId,
                    UnitCode = u.UnitCode,
                    PropertyId = u.PropertyId,
                    UnitType = u.UnitType,
                    UnitStatus = u.UnitStatus,
                    Area = u.Area,
                    MarketRent = u.MarketRent
                });

            var (data, calculatedPageSize, totalPages) = await ExecutePaginationAsync(projectedQuery, totalRecords, pageNumber, pageSize);

            return new PagedResponse<MarketRentDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber <= 0 ? 0 : pageNumber,
                PageSize = calculatedPageSize,
                TotalPages = totalPages
            };
        }

        // 7-Parameter Overload (Interface Implementation Requirement)
        public Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            string? entityId,
            string? propertyId,
            string? buildingId,
            string? unitId,
            string? leaseId,
            int pageNumber,
            int pageSize)
        {
            return GetBudgetAssumptionsAsync(
                entityId,
                propertyId,
                buildingId,
                unitId,
                leaseId,
                tenantId: null,
                pageNumber,
                pageSize);
        }

        // 8-Parameter Overload (Primary Implementation)
        public async Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            string? entityId = null,
            string? propertyId = null,
            string? buildingId = null,
            string? unitId = null,
            string? leaseId = null,
            string? tenantId = null,
            int pageNumber = 0,
            int pageSize = 10)
        {
            bool hasAnyIdPassed = !string.IsNullOrWhiteSpace(entityId) ||
                                  !string.IsNullOrWhiteSpace(propertyId) ||
                                  !string.IsNullOrWhiteSpace(buildingId) ||
                                  !string.IsNullOrWhiteSpace(unitId) ||
                                  !string.IsNullOrWhiteSpace(leaseId) ||
                                  !string.IsNullOrWhiteSpace(tenantId);

            IQueryable<PlBudgetAssumption> query;

            if (!hasAnyIdPassed)
            {
                query = _context.PlBudgetAssumptions.AsNoTracking();
            }
            else
            {
                var filteredQuery = _context.PlBudgetAssumptions.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(entityId))
                    filteredQuery = filteredQuery.Where(a => a.EntityId == entityId);

                if (!string.IsNullOrWhiteSpace(propertyId))
                    filteredQuery = filteredQuery.Where(a => a.PropertyId == propertyId);

                if (!string.IsNullOrWhiteSpace(buildingId))
                    filteredQuery = filteredQuery.Where(a => a.BuildingId == buildingId);

                if (!string.IsNullOrWhiteSpace(unitId))
                    filteredQuery = filteredQuery.Where(a => a.UnitId == unitId);

                if (!string.IsNullOrWhiteSpace(leaseId))
                    filteredQuery = filteredQuery.Where(a => a.LeaseId == leaseId);

                if (!string.IsNullOrWhiteSpace(tenantId))
                    filteredQuery = filteredQuery.Where(a => a.TenantId == tenantId);

                int matchedCount = await filteredQuery.CountAsync();

                if (matchedCount > 0)
                {
                    query = filteredQuery;
                }
                else
                {
                    query = _context.PlBudgetAssumptions.AsNoTracking()
                        .Where(a => a.EntityId == null
                                 && a.PropertyId == null
                                 && a.BuildingId == null
                                 && a.UnitId == null
                                 && a.LeaseId == null
                                 && a.TenantId == null);
                }
            }

            int totalRecords = await query.CountAsync();

            var projectedQuery = query
                .OrderBy(a => a.AssumptionId)
                .Select(a => new BudgetAssumptionDto
                {
                    AssumptionId = a.AssumptionId,
                    EntityId = a.EntityId,
                    PropertyId = a.PropertyId,
                    BuildingId = a.BuildingId,
                    UnitId = a.UnitId,
                    LeaseId = a.LeaseId,
                    TenantId = a.TenantId,
                    AssumptionName = a.AssumptionName,
                    Remarks = a.Remarks,
                    Details = a.AssumptionDetails
                        .OrderBy(d => d.SortOrder)
                        .Select(d => new BudgetAssumptionDetailDto
                        {
                            AssumptionDetailId = d.AssumptionDetailId,
                            AssumptionType = d.AssumptionType,
                            CalculationMethod = d.CalculationMethod,
                            AssumptionValue = d.AssumptionValue,
                            ValueText = d.ValueText,
                            EffectiveFrom = d.EffectiveFrom,
                            EffectiveTo = d.EffectiveTo,
                            SortOrder = d.SortOrder
                        }).ToList()
                });

            var (data, calculatedPageSize, totalPages) = await ExecutePaginationAsync(projectedQuery, totalRecords, pageNumber, pageSize);

            return new PagedResponse<BudgetAssumptionDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber <= 0 ? 0 : pageNumber,
                PageSize = calculatedPageSize,
                TotalPages = totalPages
            };
        }

        private static async Task<(List<T> Data, int PageSize, int TotalPages)> ExecutePaginationAsync<T>(
            IQueryable<T> projectedQuery,
            int totalRecords,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                var data = await projectedQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (data, pageSize, totalPages);
            }
            else
            {
                var data = await projectedQuery.ToListAsync();
                return (data, totalRecords, 1);
            }
        }
    }
}