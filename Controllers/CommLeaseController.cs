using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommLeasesController : ControllerBase
    {
        private readonly ICommLeaseRepository _leaseRepository;

        public CommLeasesController(ICommLeaseRepository leaseRepository)
        {
            _leaseRepository = leaseRepository;
        }

        // GET: api/CommLeases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommLease>>> GetLeases()
        {
            var leases = await _leaseRepository.GetAllAsync();
            return Ok(leases);
        }

        // GET: api/CommLeases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommLease>> GetLease(long id)
        {
            var lease = await _leaseRepository.GetByIdAsync(id);
            if (lease == null)
            {
                return NotFound($"Lease with ID {id} not found.");
            }
            return Ok(lease);
        }

        // GET: api/CommLeases/code/L1001
        [HttpGet("code/{leaseCode}")]
        public async Task<ActionResult<CommLease>> GetLeaseByCode(string leaseCode)
        {
            var lease = await _leaseRepository.GetByLeaseCodeAsync(leaseCode);
            if (lease == null)
            {
                return NotFound($"Lease with code {leaseCode} not found.");
            }
            return Ok(lease);
        }
    }
}
