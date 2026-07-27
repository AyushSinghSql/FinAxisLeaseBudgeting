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
    public class LeaseChargeRepository : ILeaseChargeRepository
    {
        private readonly FinAxisDbContext _context;

        public LeaseChargeRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<LeaseCharge>> GetLeaseChargesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<LeaseCharge> query = _context.LeaseCharges.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();

                // Search against text fields or parse long/decimal where applicable
                query = query.Where(lc =>
                    lc.ChargeCode.ToLower().Contains(search) ||
                    (lc.BillingFrequency != null && lc.BillingFrequency.ToLower().Contains(search)) ||
                    lc.LeaseId.ToString().Contains(search)
                );
            }

            int totalRecords = await query.CountAsync();
            List<LeaseCharge> data;
            int totalPages = 1;

            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                data = await query
                    .OrderByDescending(lc => lc.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;
                data = await query
                    .OrderByDescending(lc => lc.CreatedAt)
                    .ToListAsync();
            }

            return new PagedResponse<LeaseCharge>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<IEnumerable<LeaseChargeDropdownDto>> GetLeaseChargeDropdownAsync(string? searchTerm = null)
        {
            IQueryable<LeaseCharge> query = _context.LeaseCharges.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(lc =>
                    lc.ChargeCode.ToLower().Contains(search) ||
                    lc.LeaseId.ToString().Contains(search)
                );
            }

            return await query
                .OrderBy(lc => lc.ChargeCode)
                .Select(lc => new LeaseChargeDropdownDto
                {
                    LeaseChargeId = lc.LeaseChargeId,
                    LeaseId = lc.LeaseId,
                    ChargeCode = lc.ChargeCode
                })
                .ToListAsync();
        }
    }
}