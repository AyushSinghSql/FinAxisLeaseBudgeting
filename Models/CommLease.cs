using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("comm_leases", Schema = "public")]
    public class CommLease
    {
        [Key]
        [Column("lease_id")]
        public long LeaseId { get; set; }

        [Column("property_code")]
        [StringLength(100)]
        public string? PropertyCode { get; set; }

        [Column("lease_term")]
        public int? LeaseTerm { get; set; }

        [Required]
        [Column("lease_code")]
        [StringLength(100)]
        public string LeaseCode { get; set; } = null!;

        [Column("lease_name")]
        [StringLength(255)]
        public string? LeaseName { get; set; }

        [Column("lease_type")]
        [StringLength(100)]
        public string? LeaseType { get; set; }

        [Column("lease_start_date", TypeName = "date")]
        public DateTime? LeaseStartDate { get; set; }

        [Column("lease_end_date", TypeName = "date")]
        public DateTime? LeaseEndDate { get; set; }

        [Column("lease_move_in_date", TypeName = "date")]
        public DateTime? LeaseMoveInDate { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("customer_code")]
        [StringLength(100)]
        public string? CustomerCode { get; set; }

        [Column("company_name")]
        [StringLength(255)]
        public string? CompanyName { get; set; }

        [Column("separate_amendment_terms")]
        public int? SeparateAmendmentTerms { get; set; }

        [Column("anchor")]
        public int? Anchor { get; set; }

        [Column("holdover_percentage")]
        public decimal? HoldoverPercentage { get; set; }

        [Column("amendment_description")]
        public string? AmendmentDescription { get; set; }

        [Column("contracted_area")]
        public decimal? ContractedArea { get; set; }

        [Column("month_to_month")]
        public int? MonthToMonth { get; set; }

        [Column("due_day")]
        public int? DueDay { get; set; }

        [Column("default_sales_tran_type")]
        [StringLength(50)]
        public string? DefaultSalesTranType { get; set; }

        [Column("vat_reg_number")]
        [StringLength(100)]
        public string? VatRegNumber { get; set; }

        [Column("lease_currency")]
        [StringLength(20)]
        public string? LeaseCurrency { get; set; }

        [Column("bill_to_customer")]
        public int? BillToCustomer { get; set; }

        [Column("payment_type")]
        public int? PaymentType { get; set; }

        [Column("preferred_language")]
        public int? PreferredLanguage { get; set; }

        [Column("amendment_status")]
        public int? AmendmentStatus { get; set; }

        [Column("amendment_type")]
        public int? AmendmentType { get; set; }

        [Column("amendment_sequence")]
        public int? AmendmentSequence { get; set; }

        [Column("ext_ref_lease_id")]
        [StringLength(100)]
        public string? ExtRefLeaseId { get; set; }

        [Column("ref_property_id")]
        [StringLength(100)]
        public string? RefPropertyId { get; set; }

        [Column("ref_customer_id")]
        [StringLength(100)]
        public string? RefCustomerId { get; set; }

        [Column("primarycontact_code")]
        [StringLength(100)]
        public string? PrimaryContactCode { get; set; }

        [Column("billingcontact_code")]
        [StringLength(100)]
        public string? BillingContactCode { get; set; }

        [Column("activate")]
        public int? Activate { get; set; }

        [Column("charge_on_unpaid")]
        public int? ChargeOnUnpaid { get; set; }

        [Column("days_in_year")]
        public int? DaysInYear { get; set; }

        [Column("tenant_notes")]
        public string? TenantNotes { get; set; }

        [Column("modification_type")]
        public int? ModificationType { get; set; }

        [Column("charge_increase_type")]
        public int? ChargeIncreaseType { get; set; }

        [Column("guarantee_required")]
        public int? GuaranteeRequired { get; set; }

        [Column("is_cml_lease")]
        public int? IsCmlLease { get; set; }

        [Column("at_risk")]
        public int? AtRisk { get; set; }

        [Column("brand")]
        [StringLength(100)]
        public string? Brand { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}