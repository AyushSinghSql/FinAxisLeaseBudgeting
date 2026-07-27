using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaseBudgetController : ControllerBase
    {
        private readonly ILeaseBudgetRepository _service;

        public LeaseBudgetController(ILeaseBudgetRepository service)
        {
            _service = service;
        }

        [HttpPost("GenerateRevenueBudget")]
        public async Task<IActionResult> GenerateRevenueBudget(
            GenerateLeaseBudgetRequest request)
        {
            var result = await _service.GenerateRevenueBudgetAsync(request);

            return Ok(result);
        }
    }
}
