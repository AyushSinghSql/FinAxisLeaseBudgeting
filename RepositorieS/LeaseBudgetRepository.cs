using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FinAxisLeaseBudgeting.RepositorieS
{
    public class LeaseBudgetRepository : ILeaseBudgetRepository
    {
        private readonly FinAxisDbContext _context;
        private readonly IBudgetAssumptionRepository _budgetAssumptionRepository;

        public LeaseBudgetRepository(FinAxisDbContext context, IBudgetAssumptionRepository budgetAssumptionRepository)
        {
            _context = context;
            _budgetAssumptionRepository = budgetAssumptionRepository;
        }

        public async Task<List<PlLeaseBudget>> SearchAsync(LeaseBudgetSearchRequest request)
        {
            var query = _context.PlLeaseBudgets
                .Include(x => x.Details)
                .AsQueryable();

            query = query.Where(x => x.BudgetYear == request.BudgetYear);

            if (request.BudgetVersion.HasValue)
                query = query.Where(x => x.BudgetVersion == request.BudgetVersion.Value);

            if (!string.IsNullOrWhiteSpace(request.BudgetType))
                query = query.Where(x => x.BudgetType == request.BudgetType);

            var budgets = await query.ToListAsync();

            var result = budgets.Where(x =>
                request.Properties.Any(p =>
                    p.PropertyId == x.PropertyId &&
                    p.UnitIds == x.UnitId))
                .ToList();

            return result;

            //if (request.Properties.Any())
            //{
            //    query = query.Where(x =>
            //        request.Properties.Any(p =>
            //            p.PropertyId == x.PropertyId &&  p.UnitIds == x.UnitId));
            //}

           //return await query.ToListAsync();
        }

        public async Task<LeaseBudgetResponse> GenerateRevenueBudgetAsync_Working(
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


            var budgetId = await SaveLeaseBudgetAsync(
                response,
                request?.PropertyId,
                request.UnitId,
                leases.First().LeaseId,
                1,
                "Initial",
                "");

            return response;
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
                    x.PropertyId == request.PropertyId &&
                    x.UnitId == request.UnitId &&
                    x.LeaseStartDate.Value.Year <= request.BudgetYear &&
                    x.LeaseEndDate.Value.Year >= request.BudgetYear)
                .ToListAsync();

            if (!leases.Any())
            {

                var unit = await _context.UnitMasters
                    .FirstOrDefaultAsync(x => x.PropertyId == request.PropertyId && x.UnitId == request.UnitId);

                if (unit != null)
                {
                    response.TotalRevenue = unit.MarketRent.GetValueOrDefault();
                    leases.Add(new LeaseMaster
                    {
                        LeaseId = "N/A",
                        PropertyId = request.PropertyId,
                        UnitId = request.UnitId,
                        LeaseStartDate = new DateTime(request.BudgetYear, 1, 1),
                        LeaseEndDate = new DateTime(request.BudgetYear, 12, 31),
                        ContractRent = response.TotalRevenue
                    });
                }
            }


            foreach (var lease in leases)
            {

                //==========================================
                // Load Budget Assumptions
                //==========================================

                var assumptions =
                    await _budgetAssumptionRepository.GetAsync(
                        null,                  // Entity
                        lease.PropertyId,
                        null,                  // Building
                        lease.UnitId,
                        lease.LeaseId);


                foreach (var month in Enumerable.Range(1, 12))
                {
                    var budgetMonth =
                        new DateTime(
                            request.BudgetYear,
                            month,
                            1);


                    //==========================================
                    // Calculate Revenue Using Assumptions
                    //==========================================

                    var revenue =
                        CalculateMonthlyLeaseRevenue(
                            lease,
                            assumptions,
                            budgetMonth);



                    var monthBudget = response.MonthlyBudget
                        .FirstOrDefault(x => x.Month ==
                            CultureInfo.InvariantCulture
                            .DateTimeFormat
                            .GetAbbreviatedMonthName(month));


                    if (monthBudget == null)
                    {
                        monthBudget = new LeaseBudgetMonth
                        {
                            Month =
                            CultureInfo.InvariantCulture
                            .DateTimeFormat
                            .GetAbbreviatedMonthName(month)
                        };

                        response.MonthlyBudget.Add(monthBudget);
                    }


                    monthBudget.BaseRent += revenue.BaseRent;

                    monthBudget.CamRecovery += revenue.Cam;

                    monthBudget.TaxRecovery += revenue.Tax;

                    monthBudget.InsuranceRecovery += revenue.Insurance;

                    monthBudget.ParkingRevenue += revenue.Parking;

                    monthBudget.StorageRevenue += revenue.Storage;

                    monthBudget.PercentageRent += revenue.PercentageRent;

                    monthBudget.FreeRent += revenue.FreeRent;

                    monthBudget.BadDebt += revenue.BadDebt;


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
                }
            }


            response.TotalRevenue =
                response.MonthlyBudget
                .Sum(x => x.TotalRevenue);




            await SaveLeaseBudgetAsync(
                response,
                request.PropertyId,
                request.UnitId,
                leases.First().LeaseId,
                1,
                "Initial",
                "");


            return response;
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


        private LeaseRevenueResult CalculateMonthlyLeaseRevenue(
    LeaseMaster lease,
    BudgetAssumptionModel assumptions,
    DateTime budgetMonth)
        {
            var result = new LeaseRevenueResult();


            //-------------------------------------
            // Base Rent
            //-------------------------------------

            var months = GetInclusiveMonthDifference(DateOnly.FromDateTime(lease.LeaseStartDate.Value), DateOnly.FromDateTime(lease.LeaseEndDate.Value));

            decimal baseRent =
                lease.ContractRent.HasValue && months > 0
                ? lease.ContractRent.Value / months
                : 0;


            if (assumptions.BaseRentEscalation > 0)
            {
                baseRent +=
                    baseRent *
                    assumptions.BaseRentEscalation /
                    100;
            }



            //-------------------------------------
            // CAM
            //-------------------------------------

            decimal cam =
                lease.ChargeCode == "CAM"
                ? lease.ChargeAmount ?? 0
                : 0;


            cam +=
                cam *
                assumptions.CamGrowth /
                100;



            //-------------------------------------
            // Tax
            //-------------------------------------

            decimal tax = 0;


            tax +=
                tax *
                assumptions.TaxGrowth /
                100;



            //-------------------------------------
            // Insurance
            //-------------------------------------

            decimal insurance = 0;


            insurance +=
                insurance *
                assumptions.InsuranceGrowth /
                100;



            //-------------------------------------
            // Parking
            //-------------------------------------

            decimal parking = 0;


            parking +=
                parking *
                assumptions.ParkingGrowth /
                100;



            //-------------------------------------
            // Free Rent
            //-------------------------------------

            decimal freeRent = 0;


            if (assumptions.FreeRentMonths > 0 &&
               budgetMonth.Month <= assumptions.FreeRentMonths)
            {
                freeRent = baseRent;
            }



            //-------------------------------------
            // Bad Debt
            //-------------------------------------

            decimal grossRevenue =
                baseRent +
                cam +
                tax +
                insurance +
                parking;


            decimal badDebt =
                grossRevenue *
                assumptions.BadDebt /
                100;



            //-------------------------------------
            // Result
            //-------------------------------------

            result.BaseRent = baseRent;

            result.Cam = cam;

            result.Tax = tax;

            result.Insurance = insurance;

            result.Parking = parking;

            result.FreeRent = freeRent;

            result.BadDebt = badDebt;


            result.Total =
                grossRevenue -
                freeRent -
                badDebt;


            return result;
        }

        public async Task<long> SaveLeaseBudgetAsync(
    LeaseBudgetResponse response,
    string propertyId,
    string unitId,
    string leaseId,
    int version,
    string budgetType,
    string generatedBy)
        {
            using var tran = await _context.Database.BeginTransactionAsync();

            var budget = new PlLeaseBudget
            {
                PropertyId = propertyId,
                UnitId = unitId,
                LeaseId = leaseId,

                BudgetYear = response.BudgetYear,
                BudgetVersion = version,
                BudgetType = budgetType,

                GeneratedBy = generatedBy,
                GeneratedOn = DateTime.UtcNow,

                Status = "Draft",

                TotalBudget = response.TotalRevenue,

                CreatedAt = DateTime.UtcNow
            };

            _context.PlLeaseBudgets.Add(budget);

            await _context.SaveChangesAsync();

            foreach (var month in response.MonthlyBudget)
            {
                _context.PlLeaseBudgetDetails.Add(new PlLeaseBudgetDetail
                {
                    BudgetId = budget.BudgetId,
                    Budget = budget,
                    
                    BudgetMonth = (short)DateTime.ParseExact(
                        month.Month,
                        "MMM",
                        CultureInfo.InvariantCulture).Month,

                    BudgetYear = response.BudgetYear,

                    BaseRent = month.BaseRent,

                    CamRecovery = month.CamRecovery,

                    TaxRecovery = month.TaxRecovery,

                    InsuranceRecovery = month.InsuranceRecovery,

                    ParkingIncome = month.ParkingRevenue,

                    StorageIncome = month.StorageRevenue,

                    PercentageRent = month.PercentageRent,

                    FreeRent = month.FreeRent,

                    BadDebt = month.BadDebt,

                    TotalRevenue = month.TotalRevenue,

                    MiscIncome = 0,
                    RentAdjustment = 0,
                    RentAbatement = 0,
                    VacancyLoss = 0,
                    OccupiedDays = 0,
                    DaysInMonth = DateTime.DaysInMonth(response.BudgetYear,
                        DateTime.ParseExact(month.Month, "MMM", CultureInfo.InvariantCulture).Month),

                    ProrationFactor = 1
                });
            }

            await _context.SaveChangesAsync();

            await tran.CommitAsync();

            return budget.BudgetId;
        }


        public static int GetInclusiveMonthDifference(DateOnly startDate, DateOnly endDate)
        {
            return ((endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month) + 1;
        }

        //public RevenueCalculationResult Calculate(
        //        LeaseMaster lease,
        //        //PlLeaseRentSchedule rentSchedule,
        //        BudgetAssumptionModel assumptions,
        //        DateOnly budgetMonth)
        //{
        //    var result = new RevenueCalculationResult();

        //    //----------------------------------------------------------
        //    // Base Rent
        //    //----------------------------------------------------------

        //    //decimal baseRent = rentSchedule.MonthlyRent;
        //    decimal baseRent = lease.ContractRent.GetValueOrDefault();

        //    baseRent +=
        //        baseRent *
        //        assumptions.BaseRentEscalation /
        //        100m;

        //    //----------------------------------------------------------
        //    // CAM
        //    //----------------------------------------------------------

        //    decimal cam = lease.CamRecovery;

        //    cam +=
        //        cam *
        //        assumptions.CamGrowth /
        //        100m;

        //    //----------------------------------------------------------
        //    // Tax
        //    //----------------------------------------------------------

        //    decimal tax = lease.TaxRecovery;

        //    tax +=
        //        tax *
        //        assumptions.TaxGrowth /
        //        100m;

        //    //----------------------------------------------------------
        //    // Insurance
        //    //----------------------------------------------------------

        //    decimal insurance = lease.InsuranceRecovery;

        //    insurance +=
        //        insurance *
        //        assumptions.InsuranceGrowth /
        //        100m;

        //    //----------------------------------------------------------
        //    // Parking
        //    //----------------------------------------------------------

        //    decimal parking = lease.ParkingIncome;

        //    parking +=
        //        parking *
        //        assumptions.ParkingGrowth /
        //        100m;

        //    //----------------------------------------------------------
        //    // Storage
        //    //----------------------------------------------------------

        //    decimal storage = lease.StorageIncome;

        //    storage +=
        //        storage *
        //        assumptions.StorageGrowth /
        //        100m;

        //    //----------------------------------------------------------
        //    // Percentage Rent
        //    //----------------------------------------------------------

        //    decimal percentageRent = lease.PercentageRent;

        //    //----------------------------------------------------------
        //    // Misc
        //    //----------------------------------------------------------

        //    decimal misc = lease.MiscIncome;

        //    //----------------------------------------------------------
        //    // Free Rent
        //    //----------------------------------------------------------

        //    decimal freeRent = 0;

        //    if (assumptions.FreeRentMonths > 0)
        //    {
        //        if (budgetMonth.Month <= assumptions.FreeRentMonths)
        //        {
        //            freeRent = baseRent;
        //            baseRent = 0;
        //        }
        //    }

        //    //----------------------------------------------------------
        //    // Vacancy
        //    //----------------------------------------------------------

        //    decimal vacancy =
        //        (baseRent + cam)
        //        * assumptions.Vacancy
        //        / 100m;

        //    //----------------------------------------------------------
        //    // Bad Debt
        //    //----------------------------------------------------------

        //    decimal badDebt =
        //        (
        //            baseRent +
        //            cam +
        //            tax +
        //            insurance +
        //            parking +
        //            storage +
        //            percentageRent +
        //            misc
        //        )
        //        *
        //        assumptions.BadDebt
        //        /
        //        100m;

        //    //----------------------------------------------------------
        //    // Total
        //    //----------------------------------------------------------

        //    decimal total =
        //        baseRent +
        //        cam +
        //        tax +
        //        insurance +
        //        parking +
        //        storage +
        //        percentageRent +
        //        misc
        //        - vacancy
        //        - badDebt;

        //    //----------------------------------------------------------
        //    // Result
        //    //----------------------------------------------------------

        //    result.BaseRent = baseRent;
        //    result.CamRecovery = cam;
        //    result.TaxRecovery = tax;
        //    result.InsuranceRecovery = insurance;
        //    result.ParkingIncome = parking;
        //    result.StorageIncome = storage;
        //    result.PercentageRent = percentageRent;
        //    result.MiscIncome = misc;
        //    result.FreeRent = freeRent;
        //    result.VacancyLoss = vacancy;
        //    result.BadDebt = badDebt;
        //    result.TotalRevenue = total;

        //    //----------------------------------------------------------
        //    // Components
        //    //----------------------------------------------------------

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "BASE_RENT",
        //        Description = "Base Rent",
        //        Amount = baseRent
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "CAM",
        //        Description = "CAM Recovery",
        //        Amount = cam
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "TAX",
        //        Description = "Tax Recovery",
        //        Amount = tax
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "INSURANCE",
        //        Description = "Insurance Recovery",
        //        Amount = insurance
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "PARKING",
        //        Description = "Parking",
        //        Amount = parking
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "STORAGE",
        //        Description = "Storage",
        //        Amount = storage
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "PERCENTAGE_RENT",
        //        Description = "Percentage Rent",
        //        Amount = percentageRent
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "MISC",
        //        Description = "Misc Income",
        //        Amount = misc
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "FREE_RENT",
        //        Description = "Free Rent",
        //        Amount = -freeRent
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "VACANCY",
        //        Description = "Vacancy",
        //        Amount = -vacancy
        //    });

        //    result.Components.Add(new RevenueComponent
        //    {
        //        ComponentType = "BAD_DEBT",
        //        Description = "Bad Debt",
        //        Amount = -badDebt
        //    });

        //    return result;
        //}
    }
}
