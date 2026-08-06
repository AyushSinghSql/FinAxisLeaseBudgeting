using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPropertySecurityController : ControllerBase
    {
        private readonly UserPropertySecurityService _service;

        public UserPropertySecurityController(
            UserPropertySecurityService service)
        {
            _service = service;
        }

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> Get(long userId)
        {
            var result = await _service.GetByUserAsync(userId);

            return Ok(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(
            [FromBody] UserPropertySecurityRequest request)
        {
            await _service.UpdateUserPropertiesAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "User property security updated successfully."
            });
        }
    }
}
