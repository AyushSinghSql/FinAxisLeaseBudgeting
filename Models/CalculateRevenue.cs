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
        public decimal BaseRent { get; set; }

        public decimal Cam { get; set; }

        public decimal Tax { get; set; }

        public decimal Insurance { get; set; }

        public decimal Parking { get; set; }

        public decimal Storage { get; set; }

        public decimal PercentageRent { get; set; }

        public decimal MiscIncome { get; set; }

        public decimal FreeRent { get; set; }

        public decimal BadDebt { get; set; }

        public decimal VacancyLoss { get; set; }

        public decimal Total { get; set; }


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
