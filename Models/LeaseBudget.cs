namespace FinAxisLeaseBudgeting.Models
{
    public class LeaseRevenue
    {
        public decimal BaseRent { get; set; }

        public decimal Cam { get; set; }

        public decimal Tax { get; set; }

        public decimal Insurance { get; set; }

        public decimal Parking { get; set; }

        public decimal Storage { get; set; }

        public decimal PercentageRent { get; set; }

        public decimal FreeRent { get; set; }

        public decimal BadDebt { get; set; }
    }

    public class LeaseBudgetMonth
    {
        public string Month { get; set; }

        public decimal BaseRent { get; set; }

        public decimal CamRecovery { get; set; }

        public decimal TaxRecovery { get; set; }

        public decimal InsuranceRecovery { get; set; }

        public decimal ParkingRevenue { get; set; }

        public decimal StorageRevenue { get; set; }

        public decimal PercentageRent { get; set; }

        public decimal FreeRent { get; set; }

        public decimal BadDebt { get; set; }

        public decimal TotalRevenue { get; set; }
    }

    public class LeaseBudgetResponse
    {
        public string PropertyId { get; set; }
        public string UnitId { get; set; }

        public int BudgetYear { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<LeaseBudgetMonth> MonthlyBudget { get; set; } = new();
    }

    public class GenerateLeaseBudgetRequest
    {
        public string PropertyId { get; set; }

        public int BudgetYear { get; set; }

        public string UnitId { get; set; }  
    }
}
