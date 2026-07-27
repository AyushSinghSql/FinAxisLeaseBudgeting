using FinAxisLeaseBudgeting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanningAPI.Models;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _repository;

        public RolesController(IRoleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _repository.GetByIdAsync(id);

            if (role == null)
                return NotFound(new { Message = "Role not found." });

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            try
            {
                var result = await _repository.CreateAsync(role);

                return CreatedAtAction(nameof(GetById),
                    new { id = result.RoleId },
                    result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (id != role.RoleId)
                return BadRequest(new { Message = "Invalid Role Id." });

            try
            {
                var result = await _repository.UpdateAsync(role);

                return Ok(new
                {
                    Success = true,
                    Message = "Role updated successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { Message = "Role not found." });

                return Ok(new
                {
                    Success = true,
                    Message = "Role deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
