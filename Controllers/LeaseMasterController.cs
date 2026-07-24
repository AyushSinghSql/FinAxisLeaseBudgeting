using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaseMasterController : ControllerBase
    {
        private readonly ILeaseRepository _leaseRepository;

        public LeaseMasterController(ILeaseRepository leaseRepository)
        {
            _leaseRepository = leaseRepository;
        }

        [HttpGet("leases")]
        public async Task<IActionResult> GetLeases(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _leaseRepository.GetLeasesAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }
    }
}