using PlanningAPI.Models;

namespace FinAxisLeaseBudgeting.Interfaces
{
    public interface IReportGroupRepository
    {
        Task<List<ReportGroupDto>> GetAllAsync();

        Task<ReportGroupDto?> GetByIdAsync(int id);

        Task<ReportGroupDto> CreateAsync(SaveReportGroupDto dto);

        Task<ReportGroupDto> UpdateAsync(int id, SaveReportGroupDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
