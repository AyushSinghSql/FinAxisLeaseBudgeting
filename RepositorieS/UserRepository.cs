using FinAxisLeaseBudgeting.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlanningAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static PlanningAPI.DTO.UserMappingExtensions;

namespace PlanningAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<int> BulkDeleteAsync(List<int> id);
        Task<bool> UpdatePasswordAsync(int userId, UpdatePasswordDto dto);
        Task<bool> ResetPasswordAsync(int userId, ResetPasswordDto dto);
        Task<User> LoginAsync(LoginDto dto);
        Task<string?> GenerateTokenAsync(LoginDto dto);
        Task InsertLoginHistoryAsync(int userId, DateTime loginTime, string? ipAddress);
    }
    public class UserRepository : IUserRepository
    {
        private readonly FinAxisDbContext _context;
        private readonly IConfiguration _config;

        public UserRepository(FinAxisDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task InsertLoginHistoryAsync(

    int userId,

    DateTime loginTime,

    string? ipAddress)

        {

            var loginHistory = new UserLoginHistory
            {

                UserId = userId,

                LoginTime = loginTime,

                IpAddress = ipAddress
            };


            _context.UserLoginHistories.Add(loginHistory);

            await _context.SaveChangesAsync();

        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(p=>p.UserRole).Select(u => new User
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UserRole = u.UserRole
            }).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> AddAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.UserId);
            if (existing == null) return null;

            existing.Username = user.Username;
            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.RoleId = user.RoleId;
            existing.IsActive = user.IsActive;
            // Do not overwrite CreatedAt
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> BulkDeleteAsync(List<int> ids)
        {
            var users = await _context.Users
                .Where(u => ids.Contains(u.UserId))
                .ToListAsync();

            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync();

            return users.Count;
        }


        public async Task<bool> UpdatePasswordAsync(int userId, UpdatePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, ResetPasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        //public async Task<string?> LoginAsync(LoginDto dto)
        //{
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //        return null;

        //    return GenerateJwtToken(user);
        //}

        public async Task<string?> GenerateTokenAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

            //if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, "$2b$10$EcL9Iyd0/plq2NFcXI20P.3oYoezTLAj.HzNx501ATqc/VLY1lNPC"))
            //    return null;

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<User> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.UserRole?.RoleName ?? string.Empty)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
