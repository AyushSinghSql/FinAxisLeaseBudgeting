using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinAxisLeaseBudgeting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyMasterController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyMasterController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// GET API for dropdown searching (Returns lightweight Property ID, Code, Name)
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPropertyDropdown(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _propertyService.GetPropertyDropdownAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// GET API for retrieving full property records
        /// </summary>
        [HttpGet("properties")]
        public async Task<IActionResult> GetProperties(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var result = await _propertyService.GetPropertiesAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }
    }
}