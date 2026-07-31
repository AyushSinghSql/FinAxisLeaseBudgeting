using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("lease_master")]
    public class LeaseMaster
    {
        [Key]
        [Column("lease_id")]
        public string LeaseId { get; set; } = string.Empty;

        [Column("tenant_code")]
        public string? TenantCode { get; set; }

        [Column("tenant_name")]
        public string? TenantName { get; set; }

        [Column("property_id")]
        public string PropertyId { get; set; } = string.Empty;

        [Column("unit_id")]
        public string UnitId { get; set; } = string.Empty;

        [Column("lease_status")]
        public string? LeaseStatus { get; set; }

        [Column("lease_start_date")]
        public DateOnly? LeaseStartDate { get; set; }

        [Column("lease_end_date")]
        public DateOnly? LeaseEndDate { get; set; }

        [Column("move_in_date")]
        public DateTime? MoveInDate { get; set; }

        [Column("move_out_date")]
        public DateTime? MoveOutDate { get; set; }

        [Column("contract_rent")]
        public decimal? ContractRent { get; set; }

        [Column("charge_code")]
        public string? ChargeCode { get; set; }

        [Column("charge_amount")]
        public decimal? ChargeAmount { get; set; }

        [Column("charge_from_date")]
        public DateTime? ChargeFromDate { get; set; }

        [Column("charge_to_date")]
        public DateTime? ChargeToDate { get; set; }

        [Column("billing_frequency")]
        public string? BillingFrequency { get; set; }

        [Column("escalation_percent")]
        public decimal? EscalationPercent { get; set; }

        [Column("escalation_amount")]
        public decimal? EscalationAmount { get; set; }

        [Column("next_escalation_date")]
        public DateTime? NextEscalationDate { get; set; }

        [Column("security_deposit")]
        public decimal? SecurityDeposit { get; set; }

        [Column("renewal_probability")]
        public decimal? RenewalProbability { get; set; }

        [Column("lease_type")]
        public string? LeaseType { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
    }

    public class LeaseFilterRequest
    {
        public string? EntityId { get; set; }

        public string? PropertyId { get; set; }

        public string? UnitId { get; set; }

        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}