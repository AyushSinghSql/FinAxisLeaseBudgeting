using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using PlanningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class ReportGroupRepository : IReportGroupRepository
    {
        private readonly FinAxisDbContext _context;

        public ReportGroupRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportGroupDto>> GetAllAsync()
        {
            return await _context.ReportGroups
                .Include(x => x.Reports)
                .Select(x => new ReportGroupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Reports = x.Reports
                        .Select(r => r.ReportCode)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<ReportGroupDto?> GetByIdAsync(int id)
        {
            return await _context.ReportGroups
                .Include(x => x.Reports)
                .Where(x => x.Id == id)
                .Select(x => new ReportGroupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Reports = x.Reports
                        .Select(r => r.ReportCode)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ReportGroupDto> CreateAsync(SaveReportGroupDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var group = new ReportGroup
            {
                Name = dto.Name
            };

            _context.ReportGroups.Add(group);

            await _context.SaveChangesAsync();

            foreach (var report in dto.Reports.Distinct())
            {
                _context.ReportGroupReportMappings.Add(
                    new ReportGroupReportMapping
                    {
                        ReportGroupId = group.Id,
                        ReportCode = report
                    });
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetByIdAsync(group.Id)!;
        }

        public async Task<ReportGroupDto> UpdateAsync(int id, SaveReportGroupDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var group = await _context.ReportGroups.FindAsync(id);

            if (group == null)
                throw new Exception("Report Group not found.");

            group.Name = dto.Name;

            var existing = _context.ReportGroupReportMappings
                .Where(x => x.ReportGroupId == id);

            _context.ReportGroupReportMappings.RemoveRange(existing);

            foreach (var report in dto.Reports.Distinct())
            {
                _context.ReportGroupReportMappings.Add(
                    new ReportGroupReportMapping
                    {
                        ReportGroupId = id,
                        ReportCode = report
                    });
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetByIdAsync(id)!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var group = await _context.ReportGroups.FindAsync(id);

            if (group == null)
                return false;

            _context.ReportGroups.Remove(group);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
