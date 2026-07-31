using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.RepositorieS;
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

        [HttpPost("GenerateRevenueBudgetV1")]
        public async Task<IActionResult> GenerateRevenueBudgetV1(
    GenerateLeaseBudgetRequest request)
        {
            var result = await _service.GenerateRevenueBudgetAsyncV1(request);

            return Ok(result);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] LeaseBudgetSearchRequest request)
        {
            var result = await _service.SearchAsync(request);

            return Ok(result);
        }

        [HttpPost("BulkUpdateRevenue")]
        public async Task<IActionResult> BulkUpdateRevenue(
    BulkUpdateLeaseRevenueRequest request)
        {
            await _service.BulkUpdateRevenueAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "Revenue updated successfully."
            });
        }

        [HttpGet("{budgetId:long}")]
        public async Task<IActionResult> GetBudgetById(long budgetId)
        {
            var budget = await _service.GetBudgetByIdAsync(budgetId);

            if (budget == null)
                return NotFound($"Budget with ID {budgetId} not found.");

            return Ok(budget);
        }
    }
}
