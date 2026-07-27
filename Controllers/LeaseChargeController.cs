using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaseChargeController : ControllerBase
    {
        private readonly ILeaseChargeService _leaseChargeService;

        public LeaseChargeController(ILeaseChargeService leaseChargeService)
        {
            _leaseChargeService = leaseChargeService;
        }

        /// <summary>
        /// GET API for dropdown searching (No pagination - returns array of LeaseChargeId, LeaseId, ChargeCode)
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetLeaseChargeDropdown([FromQuery] string? searchTerm = null)
        {
            var result = await _leaseChargeService.GetLeaseChargeDropdownAsync(searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// GET API for retrieving full lease charge records with pagination
        /// </summary>
        [HttpGet("charges")]
        public async Task<IActionResult> GetLeaseCharges(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _leaseChargeService.GetLeaseChargesAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }
    }
}