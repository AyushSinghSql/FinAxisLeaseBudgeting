using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AIController(IAiService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// GET API for retrieving leases expiring in the next N months (Default: 1 month).
        /// </summary>
        [HttpGet("expiring-leases")]
        public async Task<IActionResult> GetExpiringLeases(
            [FromQuery] string? propertyId = null,
            [FromQuery] int months = 1,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _aiService.GetExpiringLeasesAsync(propertyId, months, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// GET API for retrieving vacant units for a specific property or across all properties.
        /// </summary>
        [HttpGet("vacant-units")]
        public async Task<IActionResult> GetVacantUnits(
            [FromQuery] string? propertyId = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _aiService.GetVacantUnitsAsync(propertyId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("market-rent")]
        public async Task<IActionResult> GetMarketRentUnits(
    [FromQuery] string? propertyId = null,
    [FromQuery] decimal? minRent = null,
    [FromQuery] decimal? maxRent = null,
    [FromQuery] string? unitType = null,
    [FromQuery] string? unitStatus = null,
    [FromQuery] decimal? minArea = null,
    [FromQuery] decimal? maxArea = null,
    [FromQuery] int pageNumber = 0,
    [FromQuery] int pageSize = 10)
        {
            var result = await _aiService.GetMarketRentUnitsAsync(
                propertyId, minRent, maxRent, unitType, unitStatus, minArea, maxArea, pageNumber, pageSize);

            return Ok(result);
        }

        
        [HttpGet("budget-assumptions")]
        public async Task<IActionResult> GetBudgetAssumptions(
            [FromQuery] string? entityId = null,
            [FromQuery] string? propertyId = null,
            [FromQuery] string? buildingId = null,
            [FromQuery] string? unitId = null,
            [FromQuery] string? leaseId = null,
            [FromQuery] string? tenantId = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _aiService.GetBudgetAssumptionsAsync(
                entityId, propertyId, buildingId, unitId, leaseId, pageNumber, pageSize);

            return Ok(result);
        }
    }
}