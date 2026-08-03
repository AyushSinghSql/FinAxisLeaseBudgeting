namespace FinAxisLeaseBudgeting.Models
{
    public class RevenueCalculationResult
    {
        public decimal BaseRent { get; set; }

        public decimal CamRecovery { get; set; }

        public decimal TaxRecovery { get; set; }

        public decimal InsuranceRecovery { get; set; }

        public decimal ParkingIncome { get; set; }

        public decimal StorageIncome { get; set; }

        public decimal PercentageRent { get; set; }

        public decimal MiscIncome { get; set; }

        public decimal VacancyLoss { get; set; }

        public decimal BadDebt { get; set; }

        public decimal FreeRent { get; set; }

        public decimal RentAbatement { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<RevenueComponent> Components { get; set; } = new();
    }

    public class RevenueComponent
    {
        public string ComponentType { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Amount { get; set; }
    }

    public class LeaseRevenueResult
    {
        public decimal BaseRent { get; set; } = decimal.Zero;

        public decimal Cam { get; set; } = decimal.Zero;

        public decimal UTIL { get; set; } = decimal.Zero;

        public decimal ServiceCharge { get; set; } = decimal.Zero;

        public decimal Parking { get; set; } = decimal.Zero;

        public decimal Storage { get; set; } = decimal.Zero;

        public decimal PercentageRent { get; set; } = decimal.Zero;

        public decimal MiscIncome { get; set; } = decimal.Zero;

        public decimal FreeRent { get; set; } = decimal.Zero;
        public decimal UtilityRecovery { get; set; } = decimal.Zero;
        public decimal Penalty { get; set; } = decimal.Zero;
        public decimal Deposit { get; set; } = decimal.Zero;
        public decimal Discount { get; set; } = decimal.Zero;
        public decimal Maintainance { get; set; } = decimal.Zero;
        public decimal BadDebt { get; set; } = decimal.Zero;
        public decimal Fitout { get; set; } = decimal.Zero;
        public decimal VacancyLoss { get; set; } = decimal.Zero;
        public decimal Revenue { get; set; } = decimal.Zero;

        // Optional - useful for saving component level budget
        public List<LeaseRevenueComponent> Components { get; set; }
            = new();
    }


    public class LeaseRevenueComponent
    {
        public string ComponentType { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}
