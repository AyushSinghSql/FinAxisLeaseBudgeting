using FinAxisLeaseBudgeting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanningAPI.Models;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportGroupsController : ControllerBase
    {
        private readonly IReportGroupRepository _repository;

        public ReportGroupsController(IReportGroupRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveReportGroupDto dto)
        {
            return Ok(await _repository.CreateAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SaveReportGroupDto dto)
        {
            return Ok(await _repository.UpdateAsync(id, dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _repository.DeleteAsync(id))
                return NotFound();

            return Ok(new
            {
                Message = "Deleted successfully."
            });
        }
    }
}
