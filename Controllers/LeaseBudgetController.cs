using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.RepositorieS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningAPI.Models;

namespace FinAxisLeaseBudgeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaseBudgetController : ControllerBase
    {
        private readonly ILeaseBudgetRepository _service;
        private readonly FinAxisDbContext _context;

        public LeaseBudgetController(ILeaseBudgetRepository service, FinAxisDbContext context)
        {
            _service = service;
            _context = context;
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
            var result = await _service.SearchAsyncV1(request);

            return Ok(result);
        }

        [HttpPost("SearchV2")]
        public async Task<IActionResult> SearchV2()
        {
            var result = await _service.SearchAsyncV2();

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

        //[HttpGet("GetBudgets")]
        //public async Task<IActionResult> GetBudgets([FromQuery] string? PropertyId, [FromQuery] string? UnitId)
        //{

        //    List<PlLeaseBudget> budgets = new List<PlLeaseBudget>();
        //    budgets = await _service.GetBudgetsAsync(new LeaseBudgetSearchRequest
        //    {
        //        Properties = new List<PropertyUnitSearch>
        //        {
        //            new PropertyUnitSearch
        //            {
        //                PropertyId = PropertyId,
        //                UnitIds = UnitId
        //            }
        //        }
        //    });

        //    return Ok(budgets);
        //}

        [HttpGet("GetBudgets")]
        public async Task<IActionResult> GetBudgets([FromQuery] string? PropertyId, [FromQuery] string? UnitId, [FromQuery] string? BudgetType)
        {




            List<PlLeaseBudget> budgets = new List<PlLeaseBudget>();
            var searchRequest = new LeaseBudgetSearchRequest
            {
                BudgetType = BudgetType,
                Properties = string.IsNullOrWhiteSpace(PropertyId) &&
                             string.IsNullOrWhiteSpace(UnitId)
                    ? null
                    : new List<PropertyUnitSearch>
                    {
            new PropertyUnitSearch
            {
                PropertyId = PropertyId,
                UnitIds = UnitId
            }
                    }
            };

            budgets = await _service.GetBudgetsAsync(searchRequest);

            return Ok(budgets);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBudgetAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(new { message = "Please provide at least one budget ID to delete." });
            }

            var budgets = await _context.PlLeaseBudgets
                .Include(x => x.Details)
                .Where(x => ids.Contains((int)x.BudgetId))
                .ToListAsync();

            if (!budgets.Any())
            {
                return NotFound(new { message = "No matching lease budgets were found for the provided IDs." });
            }

            var allDetails = budgets.SelectMany(b => b.Details).ToList();

            if (allDetails.Any())
            {
                _context.PlLeaseBudgetDetails.RemoveRange(allDetails);
            }

            _context.PlLeaseBudgets.RemoveRange(budgets);

            await _context.SaveChangesAsync();

            if (budgets.Count == 1)
            {
                return Ok(new { message = "Lease budget and its details deleted successfully.", deletedId = budgets.First().BudgetId });
            }

            return Ok(new { message = $"{budgets.Count} lease budgets and their details deleted successfully.", deletedIds = budgets.Select(b => b.BudgetId).ToList() });
        }

        [HttpPost("BulkUpsertDetails")]
        public async Task<IActionResult> BulkUpsertDetails([FromBody]
        List<PlLeaseBudgetDetail> request)
        {
            await _service.BulkUpsertAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "Budget details saved successfully."
            });
        }

        [HttpDelete("BulkDeleteDetails")]
        public async Task<IActionResult> BulkDeleteDetails(int BudgetId,
            [FromBody] List<string> ChargeCode)
        {
            await _service.BulkDeleteAsync(BudgetId, ChargeCode);

            return Ok();
        }

        [HttpPut("UpdateBudgetStatus")]
        public async Task<IActionResult> UpdateProjectPlan(PlLeaseBudget budget)
        {
            try
            {
                var existingPlan = await _context.PlLeaseBudgets

                        .AsNoTracking()

                        .FirstOrDefaultAsync(x => x.BudgetId == budget.BudgetId);


                if (existingPlan == null)

                    return NotFound($"Property budget with ID {budget?.BudgetId} not found.");



                var success = await _service.UpdateProperityBudgetAsync(budget);

                if (!success)

                    return NotFound($"Property budget with ID {budget?.BudgetId} not found.");

                string actionDetails;


                if (existingPlan.Status != budget.Status)

                {

                    actionDetails =

                        $"Status changed from {existingPlan.Status} to {budget.Status}";

                }

                else
                {

                    actionDetails =

                        $"{budget.BudgetType} budget updated";

                }


                await _context.SaveChangesAsync();

                //await _pl_ForecastService.CalculateRevenueCost(plan.PlId.GetValueOrDefault(), plan.TemplateId.GetValueOrDefault(), plan.Type);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
