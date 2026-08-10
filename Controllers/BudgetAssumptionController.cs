using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetAssumptionController : ControllerBase
    {
        private readonly IBudgetAssumptionRepository _repository;
        private readonly FinAxisDbContext _context;

        public BudgetAssumptionController(IBudgetAssumptionRepository repository, FinAxisDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet("load")]
        public async Task<ActionResult<BudgetAssumptionModel>> LoadAssumptions(
            [FromQuery] string? entityId,
            [FromQuery] string? propertyId,
            [FromQuery] string? unitId,
            [FromQuery] string? leaseId)
        {
            var result = await _repository.GetAsync(entityId, propertyId, unitId, leaseId);
            if (result == null) return NotFound(new { message = "No budget assumptions found." });
            return Ok(result);
        }
        [HttpGet("{assumptionId:long}")]
        public async Task<IActionResult> GetById(long assumptionId)
        {
            var result = await _repository.GetByIdAsync(assumptionId);

            if (result == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Budget assumption with ID {assumptionId} was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssumptions(
            [FromQuery] string? entityId,
            [FromQuery] string? propertyId,
            [FromQuery] string? unitId,
            [FromQuery] string? leaseId,
            [FromBody] PlBudgetAssumption modelData)
        {
            if (modelData == null) return BadRequest("Invalid payload.");
            string userId = User.Identity?.Name ?? "SYSTEM";
            await _repository.SaveOrUpdateAssumptionsAsync(entityId, propertyId, unitId, leaseId, modelData, userId);
            return Ok(new { success = true, message = "Assumptions created successfully." });
        }

        [HttpGet("scope")]
        public async Task<ActionResult<PlBudgetAssumption>> GetByExactScope(
    [FromQuery] string? entityId,
    [FromQuery] string? propertyId,
    [FromQuery] string? unitId,
    [FromQuery] string? leaseId)
        {
            var result = await _repository.GetByExactScopeAsync(entityId, propertyId, unitId, leaseId);
            if (result == null)
                return NotFound(new { message = "No budget assumption found at this scope." });
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAssumptions(
            [FromQuery] string? entityId,
            [FromQuery] string? propertyId,
            [FromQuery] string? unitId,
            [FromQuery] string? leaseId,
            [FromBody] PlBudgetAssumption modelData)
        {
            if (modelData == null) return BadRequest("Invalid payload.");
            string userId = User.Identity?.Name ?? "SYSTEM";
            await _repository.SaveOrUpdateAssumptionsAsync(entityId, propertyId, unitId, leaseId, modelData, userId);
            return Ok(new { success = true, message = "Assumptions updated successfully." });
        }

        [HttpGet("budget_assuption")]
        public async Task<IActionResult> GetBudgetAssumption(
    [FromQuery] string? entityId,
    [FromQuery] string? propertyId,
    [FromQuery] string? unitId,
    [FromQuery] string? leaseId)
        {
            var query = _context.PlBudgetAssumptions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                query = query.Where(x => x.EntityId == entityId);
            }

            if (!string.IsNullOrWhiteSpace(propertyId))
            {
                query = query.Where(x => x.PropertyId == propertyId);
            }

            if (!string.IsNullOrWhiteSpace(unitId))
            {
                query = query.Where(x => x.UnitId == unitId);
            }

            if (!string.IsNullOrWhiteSpace(leaseId))
            {
                query = query.Where(x => x.LeaseId == leaseId);
            }

            var result = await query.ToListAsync();

            return Ok(result);
        }

    }

    [ApiController]
    [Route("api/budget/lookups")]
    public class BudgetLookupController : ControllerBase
    {
        private readonly IBudgetLookupRepository _lookupRepository;

        public BudgetLookupController(IBudgetLookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        [HttpGet("entities")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetEntities()
        {
            return Ok(await _lookupRepository.GetEntitiesAsync());
        }

        [HttpGet("properties")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetProperties([FromQuery] string? entityId)
        {
            return Ok(await _lookupRepository.GetPropertiesAsync(entityId));
        }

        [HttpGet("units")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetUnits([FromQuery] string? propertyId)
        {
            return Ok(await _lookupRepository.GetUnitsAsync(propertyId));
        }

        [HttpGet("leases")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetLeases([FromQuery] string? unitId)
        {
            return Ok(await _lookupRepository.GetLeasesAsync(unitId));
        }


        [HttpGet("assumptions")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetAssumptions([FromQuery] long? assumptionId)
        {
            return Ok(await _lookupRepository.GetAssumptionsAsync(assumptionId));
        }
    }
}