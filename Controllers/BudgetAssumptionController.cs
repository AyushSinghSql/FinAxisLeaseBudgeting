//using FinAxisLeaseBudgeting.Interfaces;
//using FinAxisLeaseBudgeting.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace FinAxisLeaseBudgeting.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class BudgetAssumptionController : ControllerBase
//    {
//        private readonly IBudgetAssumptionRepository _repository;

//        public BudgetAssumptionController(
//            IBudgetAssumptionRepository repository
//            )
//        {
//            _repository = repository;
//        }

//        // GET: api/budgetassumption/load?entityId=ENTITY001&propertyId=PROPERTY100
//        [HttpGet("load")]
//        public async Task<ActionResult<BudgetAssumptionModel>> LoadAssumptions(
//            [FromQuery] string? entityId,
//            [FromQuery] string? propertyId,
//            [FromQuery] string? buildingId,
//            [FromQuery] string? unitId,
//            [FromQuery] string? leaseId)
//        {
//            var result = await _repository.GetAsync(entityId, propertyId, buildingId, unitId, leaseId);

//            if (result == null)
//            {
//                return NotFound(new { message = "No budget assumptions found for the specified scope hierarchy." });
//            }

//            return Ok(result);
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreateAssumptions(
//            [FromQuery] string? entityId,
//            [FromQuery] string? propertyId,
//            [FromQuery] string? buildingId,
//            [FromQuery] string? unitId,
//            [FromQuery] string? leaseId,
//            [FromBody] PlBudgetAssumption modelData)
//        {
//            if (modelData == null)
//            {
//                return BadRequest("Invalid assumption payload data.");
//            }

//            string userId = User.Identity?.Name ?? "SYSTEM";
//            await _repository.SaveOrUpdateAssumptionsAsync(entityId, propertyId, buildingId, unitId, leaseId, modelData, userId);

//            return Ok(new { success = true, message = "Assumptions created successfully." });
//        }

//        [HttpPut("update")]
//        public async Task<IActionResult> UpdateAssumptions(
//            [FromQuery] string? entityId,
//            [FromQuery] string? propertyId,
//            [FromQuery] string? buildingId,
//            [FromQuery] string? unitId,
//            [FromQuery] string? leaseId,
//            [FromBody] PlBudgetAssumption modelData)
//        {
//            if (modelData == null)
//            {
//                return BadRequest("Invalid assumption payload data.");
//            }

//            string userId = User.Identity?.Name ?? "SYSTEM";
//            await _repository.SaveOrUpdateAssumptionsAsync(entityId, propertyId, buildingId, unitId, leaseId, modelData, userId);

//            return Ok(new { success = true, message = "Assumptions updated successfully." });
//        }
//    }
//}


using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetAssumptionController : ControllerBase
    {
        private readonly IBudgetAssumptionRepository _repository;

        public BudgetAssumptionController(IBudgetAssumptionRepository repository)
        {
            _repository = repository;
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
    }
}