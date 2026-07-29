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
    public class UnitMasterRepository : IUnitRepository
    {
        private readonly FinAxisDbContext _context;

        public UnitMasterRepository(FinAxisDbContext context) => _context = context;

        public async Task<PagedResponse<UnitMaster>> GetUnitsAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<UnitMaster> query = _context.UnitMasters.AsNoTracking();

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.UnitCode.ToLower().Contains(search) ||
                    u.UnitType.ToLower().Contains(search) ||
                    u.UnitStatus.ToLower().Contains(search) ||
                    (u.Building != null && u.Building.ToLower().Contains(search)) ||
                    (u.Floor != null && u.Floor.ToLower().Contains(search)) ||
                    (u.Zone != null && u.Zone.ToLower().Contains(search))
                );
            }

            int totalRecords = await query.CountAsync();
            List<UnitMaster> data;
            int totalPages = 1;

            // PageNumber == 0 -> Send all records
            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                data = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;

                data = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }

            return new PagedResponse<UnitMaster>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<IEnumerable<UnitsDropdownDto>> GetUnitsDropdownAsync(string? searchTerm)
        {
            IQueryable<UnitMaster> query = _context.UnitMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.UnitCode.ToLower().Contains(search) ||
                    p.UnitCode.ToLower().Contains(search)
                );
            }

            return await query
                .OrderBy(p => p.UnitCode)
                .Select(p => new UnitsDropdownDto
                {
                    UnitId = p.UnitId,
                    UnitCode = p.UnitCode,
                    UnitName = p.UnitCode // Assuming UnitName is the same as UnitCode
                })
                .ToListAsync();
        }
    }
}