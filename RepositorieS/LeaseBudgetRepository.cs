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
            var budgets = await _context.PlLeaseBudgets
                .Include(x => x.Details)
                .ToListAsync();

            var result = budgets
                .Where(x => request.Properties.Any(p =>
                    p.PropertyId == x.PropertyId &&
                    p.UnitIds == x.UnitId))
                .ToList();

            var keys = request.Properties
                .Select(p => $"{p.PropertyId}|{p.UnitIds}")
                .ToHashSet();

            var leases = await _context.LeaseMasters
                .Where(x => keys.Contains(x.PropertyId + "|" + x.UnitId))
                .ToDictionaryAsync(x => x.PropertyId + "|" + x.UnitId, x => x.TenantCode);

            foreach (var budget in result)
            {
                var detail = budget.Details.FirstOrDefault();
                budget.TenantId = leases.GetValueOrDefault($"{budget.PropertyId}|{budget.UnitId}");
                budget.ChargeCode = detail?.ChargeCode;
                budget.AccountId = detail?.AccountId;
            }

            // Add units without budgets
            foreach (var property in request.Properties)
            {
                bool exists = result.Any(x =>
                    x.PropertyId == property.PropertyId &&
                    x.UnitId == property.UnitIds);

                if (!exists)
                {
                    result.Add(new PlLeaseBudget
                    {
                        PropertyId = property.PropertyId,
                        TenantId = leases.ContainsKey(property.UnitIds) ? leases[property.UnitIds] : string.Empty,
                        UnitId = property.UnitIds,
                        LeaseId = string.Empty,
                        BudgetYear = 0,
                        BudgetVersion = 0,
                        BudgetType = null,
                        Status = "Not Created",
                        TotalBudget = 0,
                        Details = new List<PlLeaseBudgetDetail>(),
                        ChargeCode = null,
                        AccountId = null
                    });
                }
            }

            return result;
        }
        public async Task<List<PlLeaseBudget>> SearchAsync_Working(LeaseBudgetSearchRequest request)
        {
            var query = _context.PlLeaseBudgets
                .Include(x => x.Details)
                .AsQueryable();

            var budgets = await query.ToListAsync();

            var result = budgets.Where(x =>
                request.Properties.Any(p =>
                    p.PropertyId == x.PropertyId &&
                    p.UnitIds == x.UnitId))
                .ToList();

            foreach (var budget in result)
            {
                var detail = budget.Details.FirstOrDefault();

                budget.ChargeCode = detail?.ChargeCode;
                budget.AccountId = detail?.AccountId;
            }

            return result;
        }


        public async Task<List<PlLeaseBudget>> GetBudgetsAsync(LeaseBudgetSearchRequest request)
        {
            var query = _context.PlLeaseBudgets
                .AsQueryable();

            var budgets = await query.ToListAsync();

            // Check if properties filter is null or empty. If so, return all budgets.
            if (request.Properties[0].PropertyId == null || !request.Properties.Any())
            {
                return budgets;
            }

            // Otherwise, filter by the requested properties and units
            var result = budgets.Where(x =>
                request.Properties.Any(p =>
                    p.PropertyId == x.PropertyId &&
                    p.UnitIds == x.UnitId))
                .ToList();

            return result;
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
                response.MonthlyBudget.Sum(x => x.TotalRevenue ?? 0);


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
                        LeaseStartDate = new DateOnly(request.BudgetYear, 1, 1),
                        LeaseEndDate = new DateOnly(request.BudgetYear, 12, 31),
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

                    monthBudget.TaxRecovery += revenue.UTIL;

                    monthBudget.InsuranceRecovery += revenue.ServiceCharge;

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
                .Sum(x => x.TotalRevenue ?? 0);




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



        public async Task<LeaseBudgetResponse> GenerateRevenueBudgetAsyncV1(
GenerateLeaseBudgetRequest request)
        {
            var response = new LeaseBudgetResponse
            {
                PropertyId = request.PropertyId,
                UnitId = request.UnitId
            };

            var budgetStart = request.BudgetStartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
            var budgetEnd = request.BudgetEndDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

            var LeaseStart = request.LeaseStartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
            var LeaseEnd = request.LeaseEndDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

            var ChargeCodes = await _context.ChargeCdGlAccounts
                        .AsNoTracking()
                        .Select(x => new ChargeAccountDto
                        {
                            ChargeCode = x.ChargeCode,
                            ChargeDescription = x.ChargeDescription ?? string.Empty,
                            AccountId = x.GlAccount,
                            AccountName = x.GlAccountName ?? string.Empty
                        })
                        .Distinct()
                        .OrderBy(x => x.ChargeCode)
                        .ThenBy(x => x.AccountId)
                        .ToListAsync();

            //==============================================================
            // Load all leases overlapping the budget period
            //==============================================================



            var leases = await _context.LeaseMasters
                .Where(x =>
                    x.PropertyId == request.PropertyId &&
                    x.UnitId == request.UnitId &&
                    x.LeaseStartDate <= DateOnly.FromDateTime(LeaseEnd) &&
                    x.LeaseEndDate >= DateOnly.FromDateTime(LeaseStart))
                .OrderBy(x => x.LeaseStartDate)
                .ToListAsync();

            //==============================================================
            // No Lease -> Use Market Rent
            //==============================================================

            if (!leases.Any())
            {
                var unit = await _context.UnitMasters
                    .FirstOrDefaultAsync(x =>
                        x.PropertyId == request.PropertyId &&
                        x.UnitId == request.UnitId);

                if (unit != null)
                {
                    leases.Add(new LeaseMaster
                    {
                        LeaseId = "MARKET",
                        PropertyId = request.PropertyId,
                        UnitId = request.UnitId,
                        LeaseStartDate = DateOnly.FromDateTime(budgetStart),
                        LeaseEndDate = DateOnly.FromDateTime(budgetEnd),
                        ContractRent = unit.MarketRent ?? 0
                    });
                }
            }

            //==============================================================
            // Generate Budget Month by Month
            //==============================================================

            var currentMonth = new DateOnly(
                request.LeaseStartDate?.Year ?? 0,
                request.LeaseStartDate?.Month ?? 0,
                1);

            var endMonth = new DateOnly(
                request.LeaseEndDate?.Year ?? 0,
                request.LeaseEndDate?.Month ?? 0,
                1);

            //==========================================================
            // Load Assumptions
            //==========================================================

            var assumptions =
                await _budgetAssumptionRepository.GetAsync(
                    null,
                    request.PropertyId,
                    null,
                    request.UnitId,
                    leases.FirstOrDefault()?.LeaseId);

            //==========================================================
            // Calculate Revenue
            //==========================================================

            var revenue =
                CalculateMonthlyLeaseRevenueV1(request,
                    leases.FirstOrDefault(),
                    assumptions,
                    currentMonth);

            while (currentMonth <= endMonth)
            {
                var monthBudget = new LeaseBudgetMonth();
                foreach (var charge in ChargeCodes)
                {
                    var monthStart = currentMonth.ToDateTime(TimeOnly.MinValue);

                    monthBudget = new LeaseBudgetMonth
                    {
                        BudgetMonth = (short)currentMonth.Month,
                        BudgetYear = currentMonth.Year,
                        Month = monthStart.ToString("MMM yyyy"),
                        AccountId = charge.AccountId,
                        ChargeCode = charge.ChargeCode
                    };

                    foreach (var lease in leases)
                    {
                        // Skip lease if it doesn't overlap this month
                        if (lease.LeaseStartDate > DateOnly.FromDateTime(monthStart.AddMonths(1).AddDays(-1)))
                            continue;

                        if (lease.LeaseEndDate < DateOnly.FromDateTime(monthStart))
                            continue;

                        switch (charge.ChargeCode.ToUpper())
                        {
                            case "RENT":
                                monthBudget.BaseRent = revenue.BaseRent;
                                break;
                            case "PARK":
                                monthBudget.BaseRent = revenue.Parking;
                                break;
                            case "CAM":
                                monthBudget.BaseRent = revenue.Cam;
                                break;
                            case "UTIL":
                                monthBudget.BaseRent = revenue.UTIL;
                                break;
                            case "STOR":
                                monthBudget.BaseRent = revenue.Storage;
                                break;
                            case "SERV":
                                monthBudget.BaseRent = revenue.ServiceCharge;
                                break;
                            case "PEN":
                                monthBudget.BaseRent = revenue.Penalty;
                                break;
                            case "SDDEP":
                                monthBudget.BaseRent = revenue.Deposit;
                                break;
                            case "FITOUT":
                                monthBudget.BaseRent = revenue.Fitout;
                                break;
                            case "DISC":
                                monthBudget.BaseRent = revenue.Discount;
                                break;
                            case "OTHINC":
                                monthBudget.BaseRent = revenue.MiscIncome;
                                break;
                            case "MAINT":
                                monthBudget.BaseRent = revenue.Maintainance;
                                break;
                        }
                    }
                    response.MonthlyBudget.Add(monthBudget);

                }
                currentMonth = currentMonth.AddMonths(1);
            }

            //==============================================================
            // Total Revenue
            //==============================================================

            response.TotalRevenue = response.MonthlyBudget.Sum(x => x.BaseRent);

            //==============================================================
            // Save Budget
            //==============================================================

            await SaveLeaseBudgetAsyncV1(
                response,
                request.PropertyId,
                request.UnitId,
                leases.First().LeaseId,
                1,
                request.LeaseStartDate,
                request.LeaseEndDate,
                "Initial",
                "");

            return response;
        }

        public async Task<LeaseBudgetResponse> GenerateRevenueBudgetAsyncV1_Working(
    GenerateLeaseBudgetRequest request)
        {
            var response = new LeaseBudgetResponse
            {
                PropertyId = request.PropertyId,
                UnitId = request.UnitId
            };

            var budgetStart = request.LeaseStartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
            var budgetEnd = request.LeaseEndDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

            var ChargeCodes = await _context.ChargeCdGlAccounts
                        .AsNoTracking()
                        .Select(x => new ChargeAccountDto
                        {
                            ChargeCode = x.ChargeCode,
                            ChargeDescription = x.ChargeDescription ?? string.Empty,
                            AccountId = x.GlAccount,
                            AccountName = x.GlAccountName ?? string.Empty
                        })
                        .Distinct()
                        .OrderBy(x => x.ChargeCode)
                        .ThenBy(x => x.AccountId)
                        .ToListAsync();

            //==============================================================
            // Load all leases overlapping the budget period
            //==============================================================



            var leases = await _context.LeaseMasters
                .Where(x =>
                    x.PropertyId == request.PropertyId &&
                    x.UnitId == request.UnitId &&
                    x.LeaseStartDate <= DateOnly.FromDateTime(budgetEnd) &&
                    x.LeaseEndDate >= DateOnly.FromDateTime(budgetStart))
                .OrderBy(x => x.LeaseStartDate)
                .ToListAsync();

            //==============================================================
            // No Lease -> Use Market Rent
            //==============================================================

            if (!leases.Any())
            {
                var unit = await _context.UnitMasters
                    .FirstOrDefaultAsync(x =>
                        x.PropertyId == request.PropertyId &&
                        x.UnitId == request.UnitId);

                if (unit != null)
                {
                    leases.Add(new LeaseMaster
                    {
                        LeaseId = "MARKET",
                        PropertyId = request.PropertyId,
                        UnitId = request.UnitId,
                        LeaseStartDate = DateOnly.FromDateTime(budgetStart),
                        LeaseEndDate = DateOnly.FromDateTime(budgetEnd),
                        ContractRent = unit.MarketRent ?? 0
                    });
                }
            }

            //==============================================================
            // Generate Budget Month by Month
            //==============================================================

            var currentMonth = new DateOnly(
                request.LeaseStartDate?.Year ?? 0,
                request.LeaseStartDate?.Month ?? 0,
                1);

            var endMonth = new DateOnly(
                request.LeaseEndDate?.Year ?? 0,
                request.LeaseEndDate?.Month ?? 0,
                1);

            //==========================================================
            // Load Assumptions
            //==========================================================

            var assumptions =
                await _budgetAssumptionRepository.GetAsync(
                    null,
                    request.PropertyId,
                    null,
                    request.UnitId,
                    leases.FirstOrDefault()?.LeaseId);

            //==========================================================
            // Calculate Revenue
            //==========================================================

            var revenue =
                CalculateMonthlyLeaseRevenue(
                    leases.FirstOrDefault(),
                    assumptions,
                    currentMonth.ToDateTime(TimeOnly.MinValue));

            while (currentMonth <= endMonth)
            {
                var monthBudget = new LeaseBudgetMonth();
                foreach (var charge in ChargeCodes)
                {
                    //var monthBudget = new LeaseBudgetMonth
                    //{
                    //    BudgetMonth = (short)currentMonth.Month,
                    //    BudgetYear = currentMonth.Year,
                    //    Month = currentMonth.ToString("MMM yyyy"),
                    //    AccountId = charge.AccountId,
                    //    ChargeCode = charge.ChargeCode
                    //};
                    //response.MonthlyBudget.Add(monthBudget);


                    var monthStart = currentMonth.ToDateTime(TimeOnly.MinValue);

                    monthBudget = new LeaseBudgetMonth
                    {
                        BudgetMonth = (short)currentMonth.Month,
                        BudgetYear = currentMonth.Year,
                        Month = monthStart.ToString("MMM yyyy"),
                        AccountId = charge.AccountId,
                        ChargeCode = charge.ChargeCode
                    };

                    foreach (var lease in leases)
                    {
                        // Skip lease if it doesn't overlap this month
                        if (lease.LeaseStartDate > DateOnly.FromDateTime(monthStart.AddMonths(1).AddDays(-1)))
                            continue;

                        if (lease.LeaseEndDate < DateOnly.FromDateTime(monthStart))
                            continue;

                        switch (charge.ChargeCode.ToUpper())
                        {
                            case "RENT":
                                monthBudget.BaseRent = revenue.BaseRent;
                                break;
                            case "PARK":
                                monthBudget.BaseRent = revenue.Parking;
                                break;
                            case "CAM":
                                monthBudget.BaseRent = revenue.Cam;
                                break;
                            case "UTIL":
                                monthBudget.BaseRent = revenue.UTIL;
                                break;
                            case "STOR":
                                monthBudget.BaseRent = revenue.Storage;
                                break;
                            case "SERV":
                                monthBudget.BaseRent = revenue.ServiceCharge;
                                break;
                            case "PEN":
                                monthBudget.BaseRent = revenue.Penalty;
                                break;
                            case "SDDEP":
                                monthBudget.BaseRent = revenue.Deposit;
                                break;
                            case "FITOUT":
                                monthBudget.BaseRent = revenue.Fitout;
                                break;
                            case "DISC":
                                monthBudget.BaseRent = revenue.Discount;
                                break;
                            case "OTHINC":
                                monthBudget.BaseRent = revenue.MiscIncome;
                                break;
                            case "MAINT":
                                monthBudget.BaseRent = revenue.Maintainance;
                                break;
                        }


                        //monthBudget.BaseRent += revenue.BaseRent;
                        //monthBudget.CamRecovery += revenue.Cam;
                        //monthBudget.TaxRecovery += revenue.UTIL;
                        //monthBudget.InsuranceRecovery += revenue.ServiceCharge;
                        //monthBudget.ParkingRevenue += revenue.Parking;
                        //monthBudget.StorageRevenue += revenue.Storage;
                        //monthBudget.PercentageRent += revenue.PercentageRent;
                        //monthBudget.FreeRent += revenue.FreeRent;
                        //monthBudget.BadDebt += revenue.BadDebt;


                        // Optional additional components
                        // monthBudget.MiscIncome += revenue.MiscIncome;
                        // monthBudget.VacancyLoss += revenue.VacancyLoss;
                    }
                    response.MonthlyBudget.Add(monthBudget);

                }

                //monthBudget.TotalRevenue =
                //    monthBudget.BaseRent +
                //    monthBudget.CamRecovery +
                //    monthBudget.TaxRecovery +
                //    monthBudget.InsuranceRecovery +
                //    monthBudget.ParkingRevenue +
                //    monthBudget.StorageRevenue +
                //    monthBudget.PercentageRent
                //    // + monthBudget.MiscIncome
                //    // + monthBudget.RentAdjustment
                //    - monthBudget.FreeRent
                //    // - monthBudget.RentAbatement
                //    // - monthBudget.VacancyLoss
                //    - monthBudget.BadDebt;


                currentMonth = currentMonth.AddMonths(1);
            }

            //==============================================================
            // Total Revenue
            //==============================================================

            response.TotalRevenue = response.MonthlyBudget.Sum(x => x.TotalRevenue ?? 0);

            //==============================================================
            // Save Budget
            //==============================================================

            await SaveLeaseBudgetAsyncV1(
                response,
                request.PropertyId,
                request.UnitId,
                leases.First().LeaseId,
                1,
                request.BudgetStartDate,
                request.BudgetEndDate,
                "Initial",
                "");

            return response;
        }

        private LeaseRevenue CalculateMonthlyLeaseRevenue(
    LeaseMaster lease)
        {
            // Load rent schedule, recoveries, concessions, etc.
            // Apply lease terms, escalations and proration here.


            var months = GetInclusiveMonthDifference(lease.LeaseStartDate.Value, lease.LeaseEndDate.Value);

            return new LeaseRevenue
            {
                BaseRent = lease.ContractRent.Value / months,
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


        private LeaseRevenueResult CalculateMonthlyLeaseRevenueV1(GenerateLeaseBudgetRequest request,
LeaseMaster lease,
BudgetAssumptionModel assumptions,
DateOnly budgetMonth)
        {
            var result = new LeaseRevenueResult();


            //-------------------------------------
            // Base Rent
            //-------------------------------------

            var months = GetInclusiveMonthDifference(lease.LeaseStartDate.Value, lease.LeaseEndDate.Value);

            decimal baseRent =
                lease.ContractRent.HasValue && months > 0
                ? lease.ContractRent.Value / months
                : 0;


            if (lease.LeaseId.Trim().ToUpper() == "MARKET" || budgetMonth <= lease.LeaseStartDate.Value)
            {
                result.BaseRent = baseRent;
                result.Revenue = baseRent;
                return result;
            }

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

            result.UTIL = tax;

            result.ServiceCharge = insurance;

            result.Parking = parking;

            result.FreeRent = freeRent;

            result.BadDebt = badDebt;


            result.Revenue =
                grossRevenue -
                freeRent -
                badDebt;


            return result;
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

            var months = GetInclusiveMonthDifference(lease.LeaseStartDate.Value, lease.LeaseEndDate.Value);

            decimal baseRent =
                lease.ContractRent.HasValue && months > 0
                ? lease.ContractRent.Value / months
                : 0;


            if (lease.LeaseId.Trim().ToUpper() == "MARKET")
            {
                result.BaseRent = baseRent;
                return result;
            }

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

            result.UTIL = tax;

            result.ServiceCharge = insurance;

            result.Parking = parking;

            result.FreeRent = freeRent;

            result.BadDebt = badDebt;


            result.Revenue =
                grossRevenue -
                freeRent -
                badDebt;


            return result;
        }


        public async Task BulkUpdateRevenueAsync(
BulkUpdateLeaseRevenueRequest request)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            var detailIds = request.Items
                .Select(x => x.DetailId)
                .ToList();

            var details = await _context.PlLeaseBudgetDetails
                .Where(x => detailIds.Contains(x.DetailId))
                .ToDictionaryAsync(x => x.DetailId);

            foreach (var item in request.Items)
            {
                if (!details.TryGetValue(item.DetailId, out var detail))
                    continue;

                if (item.BaseRent.HasValue)
                    detail.BaseRent = item.BaseRent.Value;

                if (item.CamRecovery.HasValue)
                    detail.CamRecovery = item.CamRecovery.Value;

                if (item.TaxRecovery.HasValue)
                    detail.TaxRecovery = item.TaxRecovery.Value;

                if (item.InsuranceRecovery.HasValue)
                    detail.InsuranceRecovery = item.InsuranceRecovery.Value;

                if (item.ParkingIncome.HasValue)
                    detail.ParkingIncome = item.ParkingIncome.Value;

                if (item.StorageIncome.HasValue)
                    detail.StorageIncome = item.StorageIncome.Value;

                if (item.PercentageRent.HasValue)
                    detail.PercentageRent = item.PercentageRent.Value;

                if (item.MiscIncome.HasValue)
                    detail.MiscIncome = item.MiscIncome.Value;

                if (item.RentAdjustment.HasValue)
                    detail.RentAdjustment = item.RentAdjustment.Value;

                if (item.FreeRent.HasValue)
                    detail.FreeRent = item.FreeRent.Value;

                if (item.RentAbatement.HasValue)
                    detail.RentAbatement = item.RentAbatement.Value;

                if (item.VacancyLoss.HasValue)
                    detail.VacancyLoss = item.VacancyLoss.Value;

                if (item.BadDebt.HasValue)
                    detail.BadDebt = item.BadDebt.Value;

                detail.TotalRevenue =
                    detail.BaseRent +
                    detail.CamRecovery +
                    detail.TaxRecovery +
                    detail.InsuranceRecovery +
                    detail.ParkingIncome +
                    detail.StorageIncome +
                    detail.PercentageRent +
                    detail.MiscIncome +
                    detail.RentAdjustment -
                    detail.FreeRent -
                    detail.RentAbatement -
                    detail.VacancyLoss -
                    detail.BadDebt;
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
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

                    CamRecovery = month.CamRecovery ?? 0,

                    TaxRecovery = month.TaxRecovery ?? 0,

                    InsuranceRecovery = month.InsuranceRecovery ?? 0,

                    ParkingIncome = month.ParkingRevenue ?? 0,

                    StorageIncome = month.StorageRevenue ?? 0,

                    PercentageRent = month.PercentageRent ?? 0,

                    FreeRent = month.FreeRent ?? 0,

                    BadDebt = month.BadDebt ?? 0,

                    TotalRevenue = month.TotalRevenue ?? 0,

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



        public async Task<long> SaveLeaseBudgetAsyncV1(
    LeaseBudgetResponse response,
    string propertyId,
    string unitId,
    string leaseId,
    int version,
    DateOnly? startDate,
    DateOnly? endDate,
    string budgetType,
    string generatedBy)
        {
            using var tran = await _context.Database.BeginTransactionAsync();


            int? lastVersion = await _context.PlLeaseBudgets.Where(p => p.PropertyId == propertyId && p.UnitId == unitId).OrderByDescending(p => p.BudgetVersion).Select(p => (int?)p.BudgetVersion).FirstOrDefaultAsync();
            version = (lastVersion ?? 0) + 1;

            var budget = new PlLeaseBudget
            {
                PropertyId = propertyId,
                UnitId = unitId,
                LeaseId = leaseId,
                TenantId = response.TenantId,
                BudgetYear = response.BudgetYear,
                BudgetVersion = version,
                BudgetType = budgetType,

                GeneratedBy = generatedBy,
                GeneratedOn = DateTime.UtcNow,
                StartDate = startDate ?? DateOnly.MinValue,
                EndDate = endDate ?? DateOnly.MaxValue,

                Status = "Draft",

                TotalBudget = response.TotalRevenue,

                CreatedAt = DateTime.UtcNow
            };

            _context.PlLeaseBudgets.Add(budget);

            await _context.SaveChangesAsync();

            foreach (var month in response.MonthlyBudget)
            {
                if (month.BaseRent == 0)
                    continue;

                _context.PlLeaseBudgetDetails.Add(new PlLeaseBudgetDetail
                {
                    BudgetId = budget.BudgetId,
                    Budget = budget,
                    AccountId = month.AccountId,
                    ChargeCode = month.ChargeCode,


                    //BudgetMonth = (short)DateTime.ParseExact(
                    //    month.Month,
                    //    "MMM",
                    //    CultureInfo.InvariantCulture).Month,
                    BudgetMonth = month.BudgetMonth.GetValueOrDefault(),

                    BudgetYear = response.BudgetYear,

                    BaseRent = month.BaseRent,

                    CamRecovery = month.CamRecovery ?? 0,

                    TaxRecovery = month.TaxRecovery ?? 0,

                    InsuranceRecovery = month.InsuranceRecovery ?? 0,

                    ParkingIncome = month.ParkingRevenue ?? 0,

                    StorageIncome = month.StorageRevenue ?? 0,

                    PercentageRent = month.PercentageRent ?? 0,

                    FreeRent = month.FreeRent ?? 0,

                    BadDebt = month.BadDebt ?? 0,

                    TotalRevenue = month.TotalRevenue ?? 0,

                    MiscIncome = 0,
                    RentAdjustment = 0,
                    RentAbatement = 0,
                    VacancyLoss = 0,
                    OccupiedDays = 0,
                    //DaysInMonth = DateTime.DaysInMonth(response.BudgetYear,
                    //    DateTime.ParseExact(month.Month, "MMM", CultureInfo.InvariantCulture).Month),
                    DaysInMonth = DateTime.DaysInMonth(month.BudgetYear.GetValueOrDefault(), month.BudgetMonth.GetValueOrDefault()),

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


        public async Task<LeaseBudgetDto?> GetBudgetByIdAsync(long budgetId)
        {
            var budget = await _context.PlLeaseBudgets
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.BudgetId == budgetId);

            if (budget == null)
                return null;

            return new LeaseBudgetDto
            {
                BudgetId = budget.BudgetId,
                PropertyId = budget.PropertyId,
                UnitId = budget.UnitId,
                LeaseId = budget.LeaseId,
                Version = budget.BudgetVersion,
                BudgetType = budget.BudgetType,
                BudgetStart = budget.StartDate,
                BudgetEnd = budget.EndDate,
                Status = budget.Status,
                Groups = budget.Details
                    .GroupBy(x => new { x.ChargeCode, x.AccountId })
                    .Select(g => new LeaseBudgetChargeGroupDto
                    {
                        ChargeCode = g.Key.ChargeCode,
                        AccountId = g.Key.AccountId,
                        Details = g.OrderBy(x => x.BudgetYear)
                                   .ThenBy(x => x.BudgetMonth)
                                   .Select(d => new LeaseBudgetDetailDto
                                   {
                                       DetailId = d.DetailId,
                                       BudgetMonth = d.BudgetMonth,
                                       BudgetYear = d.BudgetYear,
                                       BaseRent = d.BaseRent,
                                       CamRecovery = d.CamRecovery,
                                       TaxRecovery = d.TaxRecovery,
                                       InsuranceRecovery = d.InsuranceRecovery,
                                       ParkingIncome = d.ParkingIncome,
                                       StorageIncome = d.StorageIncome,
                                       PercentageRent = d.PercentageRent,
                                       MiscIncome = d.MiscIncome,
                                       RentAdjustment = d.RentAdjustment,
                                       FreeRent = d.FreeRent,
                                       RentAbatement = d.RentAbatement,
                                       VacancyLoss = d.VacancyLoss,
                                       BadDebt = d.BadDebt,
                                       TotalRevenue = d.TotalRevenue,
                                       OccupiedDays = d.OccupiedDays,
                                       DaysInMonth = d.DaysInMonth,
                                       ProrationFactor = d.ProrationFactor
                                   })
                                   .ToList()
                    })
                    .ToList()
            };
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
