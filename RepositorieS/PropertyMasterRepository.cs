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
    public class PropertyMasterRepository : IPropertyRepository
    {
        private readonly FinAxisDbContext _context;

        public PropertyMasterRepository(FinAxisDbContext context) => _context = context;

        public async Task<PagedResponse<PropertyMaster>> GetPropertiesAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<PropertyMaster> query = _context.PropertyMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.PropertyCode.ToLower().Contains(search) ||
                    p.PropertyName.ToLower().Contains(search) ||
                    p.EntityId.ToLower().Contains(search) ||
                    (p.PropertyType != null && p.PropertyType.ToLower().Contains(search)) ||
                    (p.City != null && p.City.ToLower().Contains(search)) ||
                    (p.State != null && p.State.ToLower().Contains(search)) ||
                    (p.Country != null && p.Country.ToLower().Contains(search))
                );
            }

            int totalRecords = await query.CountAsync();
            List<PropertyMaster> data;
            int totalPages = 1;

            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                data = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;
                data = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }

            return new PagedResponse<PropertyMaster>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResponse<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<PropertyMaster> query = _context.PropertyMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.PropertyCode.ToLower().Contains(search) ||
                    p.PropertyName.ToLower().Contains(search)
                );
            }

            int totalRecords = await query.CountAsync();
            List<PropertyDropdownDto> data;
            int totalPages = 1;

            var projectedQuery = query
                .OrderBy(p => p.PropertyCode)
                .Select(p => new PropertyDropdownDto
                {
                    PropertyId = p.PropertyId,
                    PropertyCode = p.PropertyCode,
                    PropertyName = p.PropertyName
                });

            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                data = await projectedQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;
                data = await projectedQuery.ToListAsync();
            }

            return new PagedResponse<PropertyDropdownDto>
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