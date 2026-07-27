using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("lease_charge")]
    public class LeaseCharge
    {
        [Key]
        [Column("lease_charge_id")]
        public long LeaseChargeId { get; set; }

        [Column("lease_id")]
        public long LeaseId { get; set; }

        [Column("charge_code")]
        public string ChargeCode { get; set; } = string.Empty;

        [Column("charge_amount")]
        public decimal ChargeAmount { get; set; }

        [Column("charge_from_date")]
        public DateTime? ChargeFromDate { get; set; }

        [Column("charge_to_date")]
        public DateTime? ChargeToDate { get; set; }

        [Column("billing_frequency")]
        public string? BillingFrequency { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class LeaseChargeDropdownDto
    {
        public long LeaseChargeId { get; set; }
        public long LeaseId { get; set; }
        public string ChargeCode { get; set; } = string.Empty;
    }
}