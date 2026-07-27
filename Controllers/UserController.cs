using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlanningAPI.DTO;
using PlanningAPI.Models;
using PlanningAPI.Repositories;
using static PlanningAPI.DTO.UserMappingExtensions;

namespace PlanningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repository.GetAllAsync();
            return Ok(users.Select(u => u.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user.ToDto());
        }

        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _repository.GetByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(user.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {

            User created = new User(); // Initialize created to avoid unassigned variable error
            try
            {
                // 🔐 Hash password before saving
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var newUser = dto.ToEntity(passwordHash);
                created = await _repository.AddAsync(newUser);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case PostgresErrorCodes.ForeignKeyViolation: // 23503
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "The selected role does not exist. Please select a valid role."
                        });

                    case PostgresErrorCodes.UniqueViolation: // 23505
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "Username already exists."
                        });

                    default:
                        return StatusCode(500, new
                        {
                            Success = false,
                            Message = "An unexpected database error occurred."
                        });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            if (id != dto.UserId) return BadRequest();

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // update fields
            existing.FullName = dto.FullName;
            existing.Email = dto.Email;
            existing.RoleId = dto.RoleId;
            existing.IsActive = dto.IsActive;

            var updated = await _repository.UpdateAsync(existing);
            return Ok(updated!.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPost("BulkDelete")]
        public async Task<IActionResult> BulkDelete(List<int> ids)
        {
            var deleted = await _repository.BulkDeleteAsync(ids);
            return NoContent();
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto dto)
        //{
        //    var token = await _repository.LoginAsync(dto);
        //    if (token == null) return Unauthorized("Invalid credentials");

        //    return Ok(new LoginResponseDto
        //    {
        //        //Token = token,
        //        Username = token.Username,
        //        User_Id = token.UserId,
        //        Role = token.Role,
        //        FullName = token.FullName
        //    });
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)

        {

            var token = await _repository.LoginAsync(dto);

            if (token == null) return Unauthorized("Invalid credentials");


            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();


            await _repository.InsertLoginHistoryAsync(

                token.UserId,

                DateTime.UtcNow,

                ipAddress);



            return Ok(new LoginResponseDto
            {

                //Token = token,        Username = token.Username,

                User_Id = token.UserId,

                Role = token.UserRole?.RoleName,

                FullName = token.FullName

            });

        }

        [HttpPost("loginV1")]
        public async Task<IActionResult> LoginV1([FromBody] LoginDto dto)
        {
            var token = await _repository.LoginAsync(dto);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new LoginResponseDto
            {
                Token = await _repository.GenerateTokenAsync(dto),
                Username = token.Username,
                User_Id = token.UserId,
                Role = token.UserRole?.RoleName,
                FullName = token.FullName
            });
        }
        [HttpPost("GenerateTokenAsync")]
        public async Task<IActionResult> GenerateTokenAsync([FromBody] LoginDto dto)
        {
            var token = await _repository.GenerateTokenAsync(dto);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(token);
        }
        [HttpPut("{id}/update-password")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
        {
            try
            {
                var success = await _repository.UpdatePasswordAsync(id, dto);
                if (!success) return NotFound("User not found");
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var success = await _repository.ResetPasswordAsync(id, dto);
            if (!success) return NotFound("User not found");
            return NoContent();
        }


    }
}
