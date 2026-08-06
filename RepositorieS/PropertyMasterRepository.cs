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
    public class PropertyMasterRepository : IPropertyRepository
    {
        private readonly FinAxisDbContext _context;

        public PropertyMasterRepository(FinAxisDbContext context)
        {
            _context = context;
        }

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

        public async Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownAsync(string? searchTerm = null)
        {
            IQueryable<PropertyMaster> query = _context.PropertyMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(p => p.EntityId.ToLower() == search);
            }

            return await query
                .OrderBy(p => p.PropertyCode)
                .Select(p => new PropertyDropdownDto
                {
                    PropertyId = p.PropertyId,
                    PropertyCode = p.PropertyCode,
                    PropertyName = p.PropertyName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyDropdownDto>> GetPropertyDropdownByUserAsync(
      int userId,
      string? searchTerm = null)
        {
            var query =
                from ups in _context.UserPropertySecurities.AsNoTracking()
                join p in _context.PropertyMasters.AsNoTracking()
                    on ups.PropertyId equals p.PropertyId
                where ups.UserId == userId
                      && ups.IsActive
                select p;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.PropertyId.ToLower().Contains(search) ||
                    p.PropertyCode.ToLower().Contains(search) ||
                    p.PropertyName.ToLower().Contains(search));
            }

            return await query
                .OrderBy(p => p.PropertyCode)
                .Select(p => new PropertyDropdownDto
                {
                    PropertyId = p.PropertyId,
                    PropertyCode = p.PropertyCode,
                    PropertyName = p.PropertyName
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<PagedResponse<PropertyBudgetDetailDto>> GetPropertyBudgetDetailsAsync(string? searchTerm = null, int pageNumber = 0, int pageSize = 10)
        {
            IQueryable<PropertyMaster> query = _context.PropertyMasters.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.PropertyId.ToLower().Contains(search) ||
                    p.PropertyCode.ToLower().Contains(search) ||
                    p.PropertyName.ToLower().Contains(search)
                );
            }

            int totalRecords = await query.CountAsync();
            List<PropertyMaster> properties;
            int totalPages = 1;

            // Standard pagination logic matching GetPropertiesAsync
            if (pageNumber > 0 && pageSize > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                properties = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                pageNumber = 0;
                pageSize = totalRecords;
                properties = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }

            // Pull units matching the paginated property IDs
            var propertyIds = properties.Select(p => p.PropertyId).ToList();
            var units = await _context.UnitMasters
                .AsNoTracking()
                .Where(u => propertyIds.Contains(u.PropertyId))
                .ToListAsync();

            var data = properties.Select(p =>
            {
                var propertyUnits = units.Where(u => u.PropertyId == p.PropertyId).ToList();

                var rentableItemsList = propertyUnits.Select((u, index) => new RentableItemDto
                {
                    Id = index + 1,
                    UnitId = u.UnitId,
                    UnitCode = u.UnitCode,
                    TypeCode = !string.IsNullOrWhiteSpace(u.UnitType) ? u.UnitType : "PARK",
                    Desc = !string.IsNullOrWhiteSpace(u.Building) ? u.Building : u.UnitCode,
                    MarketRent = u.MarketRent?.ToString("F2") ?? "150.00",
                    OccTable = "OCC_STD",
                    Items = "45",
                    ChargeCode = "PRK_CHG",
                    GlAccount = "4100-02",
                    InfMethod = "Fixed %",
                    InfTable = "INF_2026",
                    InfRate = "3.5"
                }).ToList();

                if (!rentableItemsList.Any())
                {
                    rentableItemsList.Add(new RentableItemDto
                    {
                        Id = 1,
                        TypeCode = "PARK",
                        Desc = "Covered Parking Structure",
                        MarketRent = "150.00",
                        OccTable = "OCC_STD",
                        Items = "45",
                        ChargeCode = "PRK_CHG",
                        GlAccount = "4100-02",
                        InfMethod = "Fixed %",
                        InfTable = "INF_2026",
                        InfRate = "3.5"
                    });
                }

                return new PropertyBudgetDetailDto
                {
                    Property = p.PropertyId,
                    ModelProperty = p.PropertyCode,
                    //Property = p.PropertyCode,
                    //ModelProperty = p.PropertyId,
                    PropName = p.PropertyName,
                    MarketType = !string.IsNullOrWhiteSpace(p.PropertyType) ? p.PropertyType : "Commercial",
                    AddressLine1 = p.Address ?? "",
                    City = p.City ?? "",
                    StateZip = p.State ?? "",
                    Country = !string.IsNullOrWhiteSpace(p.Country) ? p.Country : "in",
                    CurrencyArea = $"{(!string.IsNullOrWhiteSpace(p.Currency) ? p.Currency : "inr")} |",
                    LastModified = p.UpdatedAt?.ToString("MM/dd/yyyy h:mm tt") ?? "",
                    UnitStatus = propertyUnits.FirstOrDefault()?.UnitStatus ?? "Applied",
                    RentableItems = rentableItemsList
                };
            }).ToList();

            return new PagedResponse<PropertyBudgetDetailDto>
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