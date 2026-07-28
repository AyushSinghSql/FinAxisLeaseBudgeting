using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


    [Table("pl_lease_budget")]
    public class PlLeaseBudget
    {
        [Key]
        [Column("budget_id")]
        public long BudgetId { get; set; }

        [Column("property_id")]
        public string PropertyId { get; set; }

        [Column("unit_id")]
        public string UnitId { get; set; }

        [Column("lease_id")]
        public string LeaseId { get; set; }

        [Column("budget_year")]
        public int BudgetYear { get; set; }

        [Column("budget_version")]
        public int BudgetVersion { get; set; }

        [Column("budget_type")]
        public string? BudgetType { get; set; }

        [Column("tenant_id")]
        public int? TenantId { get; set; }

        [Column("generated_on")]
        public DateTime? GeneratedOn { get; set; }

        [Column("generated_by")]
        public string? GeneratedBy { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }

        [Column("total_budget")]
        public decimal TotalBudget { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PlLeaseBudgetDetail> Details { get; set; }
            = new List<PlLeaseBudgetDetail>();
    }


    [Table("pl_lease_budget_detail")]
    public class PlLeaseBudgetDetail
    {
        [Key]
        [Column("detail_id")]
        public long DetailId { get; set; }

        [Column("budget_id")]
        public long BudgetId { get; set; }

        [ForeignKey(nameof(BudgetId))]
        public PlLeaseBudget Budget { get; set; } = null!;

        [Column("budget_month")]
        public short BudgetMonth { get; set; }

        [Column("budget_year")]
        public int BudgetYear { get; set; }

        [Column("base_rent")]
        public decimal BaseRent { get; set; }

        [Column("cam_recovery")]
        public decimal CamRecovery { get; set; }

        [Column("tax_recovery")]
        public decimal TaxRecovery { get; set; }

        [Column("insurance_recovery")]
        public decimal InsuranceRecovery { get; set; }

        [Column("parking_income")]
        public decimal ParkingIncome { get; set; }

        [Column("storage_income")]
        public decimal StorageIncome { get; set; }

        [Column("percentage_rent")]
        public decimal PercentageRent { get; set; }

        [Column("misc_income")]
        public decimal MiscIncome { get; set; }

        [Column("rent_adjustment")]
        public decimal RentAdjustment { get; set; }

        [Column("free_rent")]
        public decimal FreeRent { get; set; }

        [Column("rent_abatement")]
        public decimal RentAbatement { get; set; }

        [Column("vacancy_loss")]
        public decimal VacancyLoss { get; set; }

        [Column("bad_debt")]
        public decimal BadDebt { get; set; }

        [Column("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [Column("occupied_days")]
        public int OccupiedDays { get; set; }

        [Column("days_in_month")]
        public int DaysInMonth { get; set; }

        [Column("proration_factor")]
        public decimal ProrationFactor { get; set; }
    }

    public class LeaseBudgetSearchRequest
    {
        public int BudgetYear { get; set; }

        public int? BudgetVersion { get; set; }

        public string? BudgetType { get; set; }

        public List<PropertyUnitSearch> Properties { get; set; } = new();
    }

    public class PropertyUnitSearch
    {
        public string PropertyId { get; set; }

        public string UnitIds { get; set; }
    }

    public class BulkUpdateLeaseRevenueRequest
    {
        public List<LeaseRevenueUpdateItem> Items { get; set; } = new();
    }

    public class LeaseRevenueUpdateItem
    {
        public long DetailId { get; set; }

        public decimal? BaseRent { get; set; }

        public decimal? CamRecovery { get; set; }

        public decimal? TaxRecovery { get; set; }

        public decimal? InsuranceRecovery { get; set; }

        public decimal? ParkingIncome { get; set; }

        public decimal? StorageIncome { get; set; }

        public decimal? PercentageRent { get; set; }

        public decimal? MiscIncome { get; set; }

        public decimal? RentAdjustment { get; set; }

        public decimal? FreeRent { get; set; }

        public decimal? RentAbatement { get; set; }

        public decimal? VacancyLoss { get; set; }

        public decimal? BadDebt { get; set; }
    }

}
