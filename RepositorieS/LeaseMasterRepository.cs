using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class LeaseMasterRepository : ILeaseRepository
    {
        private readonly FinAxisDbContext _context;

        public LeaseMasterRepository(FinAxisDbContext context) => _context = context;

        public async Task<PagedResponse<LeaseMaster>> GetLeasesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<LeaseMaster> query = _context.LeaseMasters.AsNoTracking();

            // 1. Search Filter across string fields
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.LeaseId.ToLower().Contains(search) ||
                    (l.TenantCode != null && l.TenantCode.ToLower().Contains(search)) ||
                    (l.TenantName != null && l.TenantName.ToLower().Contains(search)) ||
                    l.PropertyId.ToLower().Contains(search) ||
                    l.UnitId.ToLower().Contains(search) ||
                    (l.LeaseStatus != null && l.LeaseStatus.ToLower().Contains(search)) ||
                    (l.LeaseType != null && l.LeaseType.ToLower().Contains(search)) ||
                    (l.ChargeCode != null && l.ChargeCode.ToLower().Contains(search)) ||
                    (l.BillingFrequency != null && l.BillingFrequency.ToLower().Contains(search))
                );
            }

            int totalRecords = await query.CountAsync();
            List<LeaseMaster> data;
            int totalPages = 1;

            // 2. Pagination Logic (pageNumber == 0 returns all data)
            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                data = await query
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;

                data = await query
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync();
            }

            // 3. Return generic response
            return new PagedResponse<LeaseMaster>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
}