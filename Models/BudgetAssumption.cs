using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    public class BudgetAssumptionModel
    {
        public decimal BaseRentEscalation { get; set; }

        public decimal CamGrowth { get; set; }

        public decimal TaxGrowth { get; set; }

        public decimal InsuranceGrowth { get; set; }

        public decimal ParkingGrowth { get; set; }

        public decimal StorageGrowth { get; set; }

        public decimal BadDebt { get; set; }

        public decimal Vacancy { get; set; }

        public decimal MarketRentGrowth { get; set; }

        public decimal RenewalIncrease { get; set; }

        public decimal RenewalProbability { get; set; }

        public int FreeRentMonths { get; set; }
    }

    [Table("pl_budget_assumption")]
    public class PlBudgetAssumption
    {
        [Key]
        [Column("assumption_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AssumptionId { get; set; }

        [StringLength(50)]
        [Column("entity_id")]
        public string? EntityId { get; set; }

        [StringLength(50)]
        [Column("property_id")]
        public string? PropertyId { get; set; }

        [StringLength(50)]
        [Column("building_id")]
        public string? BuildingId { get; set; }

        [StringLength(50)]
        [Column("unit_id")]
        public string? UnitId { get; set; }

        [StringLength(50)]
        [Column("lease_id")]
        public string? LeaseId { get; set; }

        [StringLength(50)]
        [Column("tenant_id")]
        public string? TenantId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("assumption_name")]
        public string AssumptionName { get; set; } = string.Empty;

        [Column("remarks")]
        public string? Remarks { get; set; }

        [StringLength(100)]
        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        // Navigation Property
        public virtual ICollection<PlBudgetAssumptionDetail> AssumptionDetails { get; set; }
            = new List<PlBudgetAssumptionDetail>();
    }

    [Table("pl_budget_assumption_detail")]
    public class PlBudgetAssumptionDetail
    {
        [Key]
        [Column("assumption_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AssumptionDetailId { get; set; }

        [Required]
        [Column("assumption_id")]
        public long AssumptionId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("assumption_type")]
        public string AssumptionType { get; set; } = string.Empty;

        //[Required]
        [StringLength(30)]
        [Column("calculation_method")]
        public string CalculationMethod { get; set; } 

        [Column("assumption_value", TypeName = "numeric(18,4)")]
        public decimal? AssumptionValue { get; set; }

        [StringLength(100)]
        [Column("value_text")]
        public string? ValueText { get; set; }

        [Column("effective_from")]
        public DateOnly? EffectiveFrom { get; set; }

        [Column("effective_to")]
        public DateOnly? EffectiveTo { get; set; }

        [Column("sort_order")]
        public short SortOrder { get; set; } = 1;

        [StringLength(100)]
        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        // Navigation Property
        [ForeignKey(nameof(AssumptionId))]
        public virtual PlBudgetAssumption? BudgetAssumption { get; set; }
    }

    public class LookupItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

}
