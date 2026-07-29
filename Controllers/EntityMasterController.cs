using FinAxisLeaseBudgeting.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningAPI.Models;

namespace PlanningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityMasterController : ControllerBase
    {
        private readonly FinAxisDbContext _context;

        public EntityMasterController(FinAxisDbContext context)
        {
            _context = context;
        }

        // GET: api/EntityMaster
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityMaster>>> GetAll()
        {
            return await _context.EntityMasters
                .OrderBy(x => x.EntityName)
                .ToListAsync();
        }

        [HttpGet("Dropdown")]
        public async Task<ActionResult> GetEntityDropdown()
        {
            var result = await _context.EntityMasters
                .AsNoTracking()
                .OrderBy(x => x.EntityName)
                .Select(x => new 
                {
                    Id = x.EntityId,
                    Name = x.EntityName
                })
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/EntityMaster/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityMaster>> Get(string id)
        {
            var entity = await _context.EntityMasters
                .FirstOrDefaultAsync(x => x.EntityId == id);

            if (entity == null)
                return NotFound();

            return entity;
        }

        // POST: api/EntityMaster
        [HttpPost]
        public async Task<ActionResult<EntityMaster>> Create(EntityMaster model)
        {
            if (await _context.EntityMasters.AnyAsync(x => x.EntityCode == model.EntityCode))
                return BadRequest("Entity Code already exists.");

            model.CreatedAt = DateTime.UtcNow;

            _context.EntityMasters.Add(model);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.EntityId }, model);
        }

        // PUT: api/EntityMaster/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, EntityMaster model)
        {
            if (id != model.EntityId)
                return BadRequest();

            var entity = await _context.EntityMasters.FindAsync(id);

            if (entity == null)
                return NotFound();

            if (await _context.EntityMasters.AnyAsync(x =>
                    x.EntityCode == model.EntityCode &&
                    x.EntityId != id))
            {
                return BadRequest("Entity Code already exists.");
            }

            entity.EntityCode = model.EntityCode;
            entity.EntityName = model.EntityName;
            entity.BaseCurrency = model.BaseCurrency;
            entity.Country = model.Country;
            entity.Region = model.Region;
            entity.OwnershipGroup = model.OwnershipGroup;
            entity.ParentEntityId = model.ParentEntityId;
            entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = model.UpdatedBy;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EntityMaster/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.EntityMasters.FindAsync(id);

            if (entity == null)
                return NotFound();

            _context.EntityMasters.Remove(entity);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}