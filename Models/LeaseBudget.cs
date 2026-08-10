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
        public short? BudgetMonth { get; set; }

        public int? BudgetYear { get; set; }
        public string? Month { get; set; }
        public string? AccountId { get; set; }
        public string? ChargeCode { get; set; }

        public decimal BaseRent { get; set; }

        public decimal? CamRecovery { get; set; }

        public decimal? TaxRecovery { get; set; }

        public decimal? InsuranceRecovery { get; set; }

        public decimal? ParkingRevenue { get; set; }

        public decimal? StorageRevenue { get; set; }

        public decimal? PercentageRent { get; set; }

        public decimal? FreeRent { get; set; }

        public decimal? BadDebt { get; set; }

        public decimal? TotalRevenue { get; set; }


    }

    public class LeaseBudgetResponse
    {
        public string PropertyId { get; set; }
        public string UnitId { get; set; }
        public string TenantId { get; set; }

        public int BudgetYear { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<LeaseBudgetMonth> MonthlyBudget { get; set; } = new();
    }

    public class GenerateLeaseBudgetRequest
    {
        public string PropertyId { get; set; }
        public string BudgetType { get; set; }

        public int BudgetYear { get; set; }
        public int AssumptionId { get; set; }

        public string UnitId { get; set; }

        public DateOnly? LeaseStartDate { get; set; }

        public DateOnly? LeaseEndDate { get; set; }

        public DateOnly? BudgetStartDate { get; set; }

        public DateOnly? BudgetEndDate { get; set; }
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
        public string? TenantId { get; set; }

        [Column("generated_on")]
        public DateTime? GeneratedOn { get; set; }

        [Column("generated_by")]
        public string? GeneratedBy { get; set; }

        [Column("final_version")]
        public bool? FinalVersion { get; set; }
        [Column("is_completed")]
        public bool? IsCompleted { get; set; }
        [Column("is_approved")]
        public bool? IsApproved { get; set; }


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

        [Column("budget_start_date")]
        public DateOnly StartDate { get; set; }

        [Column("budget_end_date")]
        public DateOnly EndDate { get; set; }

        [Column("assumption_id")]
        public long? AssumptionId { get; set; }
        [Column("is_manual")]
        public bool IsManual { get; set; }

        [Column("revenue_source")]
        [StringLength(30)]
        public string? RevenueSource { get; set; }

        [NotMapped]
        public string? ChargeCode { get; set; }

        [NotMapped]
        public string? AccountId { get; set; }

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

        [Column("charge_code")]
        [StringLength(50)]
        public string? ChargeCode { get; set; }

        [Column("account_id")]
        [StringLength(50)]
        public string? AccountId { get; set; }
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

    //public class LeaseBudgetDetailDto
    //{
    //    public short BudgetMonth { get; set; }

    //    public int BudgetYear { get; set; }

    //    public string ChargeCode { get; set; } = string.Empty;

    //    public string AccountId { get; set; } = string.Empty;

    //    public decimal Amount { get; set; }
    //}

    public class LeaseBudgetDetailDto
    {
        public long DetailId { get; set; }
        public short BudgetMonth { get; set; }
        public int BudgetYear { get; set; }
        public decimal BaseRent { get; set; }
        //public decimal CamRecovery { get; set; }
        //public decimal TaxRecovery { get; set; }
        //public decimal InsuranceRecovery { get; set; }
        //public decimal ParkingIncome { get; set; }
        //public decimal StorageIncome { get; set; }
        //public decimal PercentageRent { get; set; }
        //public decimal MiscIncome { get; set; }
        //public decimal RentAdjustment { get; set; }
        //public decimal FreeRent { get; set; }
        //public decimal RentAbatement { get; set; }
        //public decimal VacancyLoss { get; set; }
        //public decimal BadDebt { get; set; }
        //public decimal TotalRevenue { get; set; }
        //public int OccupiedDays { get; set; }
        //public int DaysInMonth { get; set; }
        //public decimal ProrationFactor { get; set; }
        public string? ChargeCode { get; set; }
        public string? AccountId { get; set; }
    }


    public class LeaseBudgetDetailBulkRequest
    {
        public long BudgetId { get; set; }

        public List<LeaseBudgetDetailDto> Details { get; set; } = new();
    }


    public class BudgetChargeGroup
    {
        public string ChargeCode { get; set; }
        public string AccountId { get; set; }
        public List<PlLeaseBudgetDetail> Details { get; set; }
    }

    public class LeaseBudgetDto
    {
        public long BudgetId { get; set; }
        public string PropertyId { get; set; }
        public string UnitId { get; set; }
        public string LeaseId { get; set; }
        public int Version { get; set; }
        public string BudgetType { get; set; }
        public DateOnly BudgetStart { get; set; }
        public DateOnly BudgetEnd { get; set; }
        public string Status { get; set; }

        public List<LeaseBudgetChargeGroupDto> Groups { get; set; } = new();
    }

    public class LeaseBudgetChargeGroupDto
    {
        public string ChargeCode { get; set; }
        public string AccountId { get; set; }
        public List<LeaseBudgetDetailDto> Details { get; set; } = new();
    }

    
}
