using System.Collections.Generic;
using System.Threading.Tasks;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;

namespace FinAxisLeaseBudgeting.Services
{
    public interface ILeaseChargeService
    {
        Task<PagedResponse<LeaseCharge>> GetLeaseChargesAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<LeaseChargeDropdownDto>> GetLeaseChargeDropdownAsync(string? searchTerm);
    }

    public class LeaseChargeService : ILeaseChargeService
    {
        private readonly ILeaseChargeRepository _leaseChargeRepository;

        public LeaseChargeService(ILeaseChargeRepository leaseChargeRepository)
        {
            _leaseChargeRepository = leaseChargeRepository;
        }

        public async Task<PagedResponse<LeaseCharge>> GetLeaseChargesAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            return await _leaseChargeRepository.GetLeaseChargesAsync(searchTerm, pageNumber, pageSize);
        }

        public async Task<IEnumerable<LeaseChargeDropdownDto>> GetLeaseChargeDropdownAsync(string? searchTerm)
        {
            return await _leaseChargeRepository.GetLeaseChargeDropdownAsync(searchTerm);
        }
    }
}