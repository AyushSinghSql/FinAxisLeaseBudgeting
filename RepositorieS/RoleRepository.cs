namespace FinAxisLeaseBudgeting.RepositorieS
{
    using FinAxisLeaseBudgeting.Data;
    using FinAxisLeaseBudgeting.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;
    using PlanningAPI.Models;
    using System;

    public class RoleRepository : IRoleRepository
    {
        private readonly FinAxisDbContext _context;

        public RoleRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .OrderBy(x => x.RoleName)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return role;
            }
            catch (DbUpdateException ex)
            {
                throw HandleDbException(ex);
            }
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            try
            {
                _context.Roles.Update(role);
                await _context.SaveChangesAsync();

                return role;
            }
            catch (DbUpdateException ex)
            {
                throw HandleDbException(ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
                return false;

            try
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                throw HandleDbException(ex);
            }
        }

        private Exception HandleDbException(DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        return new Exception("Role name already exists.");

                    case PostgresErrorCodes.ForeignKeyViolation:
                        return new Exception("Role cannot be deleted because it is assigned to one or more users.");

                    case PostgresErrorCodes.NotNullViolation:
                        return new Exception("Required fields are missing.");

                    default:
                        return new Exception(pgEx.MessageText);
                }
            }

            return ex;
        }
    }
}
