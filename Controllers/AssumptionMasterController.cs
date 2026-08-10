using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssumptionMasterController : ControllerBase
    {
        private readonly FinAxisDbContext _context;

        public AssumptionMasterController(FinAxisDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssumptionType>>> GetAllAssumptionTypes()
        {
            return await _context.AssumptionTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssumptionType>> GetAssumptionTypeById(int id)
        {
            var assumptionType = await _context.AssumptionTypes.FindAsync(id);

            if (assumptionType == null)
            {
                return NotFound();
            }

            return assumptionType;
        }

        [HttpPost]
        public async Task<ActionResult<AssumptionType>> CreateAssumptionType([FromBody] AssumptionType assumptionType)
        {
            assumptionType.CreatedAt = DateTime.UtcNow;

            _context.AssumptionTypes.Add(assumptionType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssumptionTypeById), new { id = assumptionType.Id }, assumptionType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssumptionType(int id, [FromBody] AssumptionType assumptionType)
        {
            if (id != assumptionType.Id)
            {
                return BadRequest("ID mismatch.");
            }

            _context.Entry(assumptionType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.AssumptionTypes.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssumptionType(int id)
        {
            var assumptionType = await _context.AssumptionTypes.FindAsync(id);
            if (assumptionType == null)
            {
                return NotFound();
            }

            _context.AssumptionTypes.Remove(assumptionType);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
