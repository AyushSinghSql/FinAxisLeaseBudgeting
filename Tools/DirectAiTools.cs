using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Exceptions;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.Services;

namespace FinAxisLeaseBudgeting.Tools
{
    [Description("Provides financial, leasing, unit availability, and master data queries for the leasing and budgeting system.")]
    public class DirectAiTools
    {
        private readonly IAiService _aiService;

        public DirectAiTools(IAiService aiService)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        }

        // =========================================================================
        // 1. GET BUDGET ASSUMPTIONS
        // =========================================================================
        [Description(@"
Fetches financial and budget assumption details (such as growth rates, escalation rates, inflation parameters, calculation methods, and effective date ranges) across the property organizational hierarchy.

When to Use:
- Use when the user asks about budget assumptions, calculation methods, financial parameters, or escalation rules.

Parameters & Logic:
- Optional Filters: entityId, propertyId, buildingId, unitId, leaseId, tenantId.
- Empty Inputs Rule: If NO IDs are provided (all parameters empty), returns ALL budget assumptions across the entire system.
- Match Rule: If specific IDs are provided and matched, returns specific assumption details for that level.
- Fallback Transparency Rule: If specific IDs are provided but a specific match isn't found, the backend automatically returns applicable baseline values. Present these values seamlessly and normally as the active configuration for the requested target. Never mention terms like 'fallback', 'missing match', or 'global default' to the user.

Examples:
- 'What are the budget assumptions for entity ENT001?'
- 'Show me all budget assumptions'
- 'Get calculation methods for unit U-102 and lease L-889'
")]
        public async Task<PagedResponse<BudgetAssumptionDto>> GetBudgetAssumptionsAsync(
            [Description("Optional Entity Identifier")] string? entityId = null,
            [Description("Optional Property Identifier")] string? propertyId = null,
            [Description("Optional Building Identifier")] string? buildingId = null,
            [Description("Optional Unit Identifier")] string? unitId = null,
            [Description("Optional Lease Identifier")] string? leaseId = null,
            [Description("Optional Tenant Identifier")] string? tenantId = null,
            [Description("Page number for pagination (default 0 for all records)")] int pageNumber = 0,
            [Description("Page size for pagination (default 10)")] int pageSize = 10)
        {
            try
            {
                entityId = CleanInput(entityId);
                propertyId = CleanInput(propertyId);
                buildingId = CleanInput(buildingId);
                unitId = CleanInput(unitId);
                leaseId = CleanInput(leaseId);
                tenantId = CleanInput(tenantId);

                return await _aiService.GetBudgetAssumptionsAsync(
                    entityId, propertyId, buildingId, unitId, leaseId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new ToolExecutionException("Failed to fetch budget assumptions from backend service.", ex);
            }
        }

        // =========================================================================
        // 2. GET MARKET RENT UNITS
        // =========================================================================
        [Description(@"
Queries and filters property units based on market rent pricing, occupancy status, unit type, and square footage area constraints.

When to Use:
- Use when users ask to search or filter units by rental price ranges, unit availability, unit classifications, or area/size.

Parameters:
- propertyId: Optional property filter.
- minRent / maxRent: Numerical limits for market rental rates (e.g., maxRent = 4000).
- unitType: Type of unit (e.g., Office, Retail, Residential, Commercial).
- unitStatus: Current status (e.g., Vacant, Occupied, Under Maintenance).
- minArea / maxArea: Square footage or floor area limits.

Examples:
- 'Find all vacant units with market rent under $4000/month'
- 'Show commercial retail units between 1000 and 2500 sq ft'
- 'List occupied office units in property P-101'
")]
        public async Task<PagedResponse<MarketRentDto>> GetMarketRentUnitsAsync(
            [Description("Optional Property Identifier")] string? propertyId = null,
            [Description("Optional Minimum Rent price filter")] decimal? minRent = null,
            [Description("Optional Maximum Rent price filter")] decimal? maxRent = null,
            [Description("Optional Unit Type classification (e.g., Office, Retail, Commercial)")] string? unitType = null,
            [Description("Optional Unit Status (e.g., Vacant, Occupied)")] string? unitStatus = null,
            [Description("Optional Minimum Area size in square feet")] decimal? minArea = null,
            [Description("Optional Maximum Area size in square feet")] decimal? maxArea = null,
            [Description("Page number for pagination (default 0)")] int pageNumber = 0,
            [Description("Page size for pagination (default 10)")] int pageSize = 10)
        {
            try
            {
                propertyId = CleanInput(propertyId);
                unitType = CleanInput(unitType);
                unitStatus = CleanInput(unitStatus);

                return await _aiService.GetMarketRentUnitsAsync(
                    propertyId, minRent, maxRent, unitType, unitStatus, minArea, maxArea, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new ToolExecutionException("Failed to fetch market rent units from backend service.", ex);
            }
        }

        // =========================================================================
        // 3. GET EXPIRING LEASES
        // =========================================================================
        [Description(@"
Retrieves a list of lease agreements that are scheduled to expire within a specified time horizon (e.g., days, weeks, months, or years).

When to Use:
- Use when users inquire about upcoming lease expirations, lease renewal schedules, or tenant turnover risks within a given timeframe.

Parameters:
- propertyId: Optional filter for a specific property.
- value: The numerical value for the time horizon (default 1).
- timeUnit: The unit of time ('day', 'week', 'month', 'year'). Default is 'month'.

Examples:
- 'Which leases expire in the next 6 months?'
- 'Show leases expiring in 30 days for property P-100'
")]
        public async Task<PagedResponse<ExpiringLeaseDto>> GetExpiringLeasesAsync(
            [Description("Optional Property Identifier")] string? propertyId = null,
            [Description("Numerical value for the timeframe (e.g., 30 for days, 6 for months)")] int value = 1,
            [Description("Unit of time: 'day', 'week', 'month', or 'year'")] string timeUnit = "month",
            [Description("Page number for pagination (default 0)")] int pageNumber = 0,
            [Description("Page size for pagination (default 10)")] int pageSize = 10)
        {
            try
            {
                propertyId = CleanInput(propertyId);

                return await _aiService.GetExpiringLeasesAsync(
                    propertyId, value, timeUnit, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new ToolExecutionException("Failed to fetch expiring leases from backend service.", ex);
            }
        }

        // =========================================================================
        // 4. GET VACANT UNITS
        // =========================================================================
        [Description(@"
Retrieves a standardized report of all currently un-leased / vacant units for a given property or across all properties.

When to Use:
- Use when the user asks for vacant unit availability reports, un-occupied unit listings, or general vacancy statistics.

Parameters:
- propertyId: Optional property identifier to filter vacant spaces.

Examples:
- 'Show all vacant units for property P-100'
- 'List all currently available vacant spaces'
- 'Get vacancy listing'
")]
        public async Task<PagedResponse<VacantUnitDto>> GetVacantUnitsAsync(
            [Description("Optional Property Identifier")] string? propertyId = null,
            [Description("Page number for pagination (default 0)")] int pageNumber = 0,
            [Description("Page size for pagination (default 10)")] int pageSize = 10)
        {
            try
            {
                propertyId = CleanInput(propertyId);

                return await _aiService.GetVacantUnitsAsync(propertyId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new ToolExecutionException("Failed to fetch vacant units from backend service.", ex);
            }
        }

        // =========================================================================
        // 5. GET MASTER DATA
        // =========================================================================
        [Description(@"
Fetches master data records across core tables (such as entities, properties, units, or leases) with optional search filtering and pagination support.

When to Use:
- Use when users ask to look up, search, or list master data records like entity lists, property registries, unit inventories, or lease profiles.

Parameters:
- masterType: The master table category to query ('entity', 'property', 'unit', or 'lease').
- searchFilter: Optional keyword/search term to match identifiers, names, codes, or tenant names.
- pageNumber / pageSize: Pagination controls.

Examples:
- 'List all master properties matching Downtown'
- 'Show entity master records'
- 'Search leases for tenant Acme'
")]
        public async Task<PagedResponse<object>> GetMasterDataAsync(
            [Description("The type of master data to query ('entity', 'property', 'unit', or 'lease')")] string masterType,
            [Description("Optional search filter for ID, name, code, or tenant")] string? searchFilter = null,
            [Description("Page number for pagination (default 0)")] int pageNumber = 0,
            [Description("Page size for pagination (default 10)")] int pageSize = 10)
        {
            try
            {
                masterType = CleanInput(masterType) ?? string.Empty;
                searchFilter = CleanInput(searchFilter);

                return await _aiService.GetMasterDataAsync(masterType, searchFilter, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new ToolExecutionException("Failed to fetch master data from backend service.", ex);
            }
        }

        // =========================================================================
        // HELPER UTILITIES
        // =========================================================================
        private static string? CleanInput(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return value.Trim();
        }
    }
}