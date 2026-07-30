using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.RepositorieS;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitMasterController : ControllerBase
    {
        private readonly IUnitRepository _IUnitRepository;

        public UnitMasterController(IUnitRepository UnitRepository)
        {
            _IUnitRepository = UnitRepository;
        }


        [HttpGet("units")]
        public async Task<IActionResult> GetUnits(
             [FromQuery] string? searchTerm = null,
             [FromQuery] int pageNumber = 0,
             [FromQuery] int pageSize = 10)
        {
            var result = await _IUnitRepository.GetUnitsAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetUnitsDropdown([FromQuery] string? searchTerm = null)
        {
            var result = await _IUnitRepository.GetUnitsDropdownAsync(searchTerm);
            return Ok(result);
        }

    }
}