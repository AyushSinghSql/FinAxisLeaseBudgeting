using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class LeaseBudgetRepository : ILeaseBudgetRepository
    {
        private readonly FinAxisDbContext _context;

        public LeaseBudgetRepository(FinAxisDbContext context)
        {
            _context = context;
        }

        public async Task<LeaseBudgetResponse> GenerateRevenueBudgetAsync(
            GenerateLeaseBudgetRequest request)
        {
            var response = new LeaseBudgetResponse
            {
                PropertyId = request.PropertyId,
                BudgetYear = request.BudgetYear,
                UnitId = request.UnitId
            };

            var leases = await _context.LeaseMasters
                .Where(x =>
                    x.PropertyId == request.PropertyId && x.UnitId == request.UnitId &&
                    x.LeaseStartDate.Value.Year <= request.BudgetYear &&
                    x.LeaseEndDate.Value.Year >= request.BudgetYear)
                .ToListAsync();

            foreach (var lease in leases)
            {
                var revenue = CalculateMonthlyLeaseRevenue(
                    lease);


                foreach (var month in Enumerable.Range(1, 12))
                {
                    var monthBudget = new LeaseBudgetMonth
                    {
                        Month = CultureInfo.InvariantCulture.DateTimeFormat
                            .GetAbbreviatedMonthName(month)
                    };

                    //foreach (var lease in leases)
                    {
                        //var revenue = CalculateLeaseRevenue(
                        //    lease,
                        //    request.BudgetYear,
                        //    month);

                        monthBudget.BaseRent += revenue.BaseRent;
                        monthBudget.CamRecovery += revenue.Cam;
                        monthBudget.TaxRecovery += revenue.Tax;
                        monthBudget.InsuranceRecovery += revenue.Insurance;
                        monthBudget.ParkingRevenue += revenue.Parking;
                        monthBudget.StorageRevenue += revenue.Storage;
                        monthBudget.PercentageRent += revenue.PercentageRent;
                        monthBudget.FreeRent += revenue.FreeRent;
                        monthBudget.BadDebt += revenue.BadDebt;
                    }

                    monthBudget.TotalRevenue =
                        monthBudget.BaseRent +
                        monthBudget.CamRecovery +
                        monthBudget.TaxRecovery +
                        monthBudget.InsuranceRecovery +
                        monthBudget.ParkingRevenue +
                        monthBudget.StorageRevenue +
                        monthBudget.PercentageRent -
                        monthBudget.FreeRent -
                        monthBudget.BadDebt;

                    response.MonthlyBudget.Add(monthBudget);
                }

            }
            response.TotalRevenue =
                response.MonthlyBudget.Sum(x => x.TotalRevenue);

            return response;
        }

        private LeaseRevenue CalculateLeaseRevenue(
            LeaseMaster lease,
            int year,
            int month)
        {
            // Load rent schedule, recoveries, concessions, etc.
            // Apply lease terms, escalations and proration here.

            return new LeaseRevenue
            {
                BaseRent = lease.ContractRent.Value,
                Cam = 0,
                Tax = 0,
                Insurance = 0,
                Parking = 0,
                Storage = 0,
                PercentageRent = 0,
                FreeRent = 0,
                BadDebt = 0
            };
        }

        private LeaseRevenue CalculateMonthlyLeaseRevenue(
    LeaseMaster lease)
        {
            // Load rent schedule, recoveries, concessions, etc.
            // Apply lease terms, escalations and proration here.


            var months = GetInclusiveMonthDifference(DateOnly.FromDateTime(lease.LeaseStartDate.Value), DateOnly.FromDateTime(lease.LeaseEndDate.Value));

            return new LeaseRevenue
            {
                BaseRent = lease.ContractRent.Value/months,
                Cam = 0,
                Tax = 0,
                Insurance = 0,
                Parking = 0,
                Storage = 0,
                PercentageRent = 0,
                FreeRent = 0,
                BadDebt = 0
            };
        }


        public static int GetInclusiveMonthDifference(DateOnly startDate, DateOnly endDate)
        {
            return ((endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month) + 1;
        }
    }
}
