using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("comm_charge_schedules", Schema = "public")]
    public class CommChargeSchedule
    {
        [Key]
        [Column("charge_schedule_id")]
        public long ChargeScheduleId { get; set; }

        [Column("property_code")]
        [StringLength(100)]
        public string? PropertyCode { get; set; }

        [Column("lease_code")]
        [StringLength(100)]
        public string? LeaseCode { get; set; }

        [Column("rate_code")]
        [StringLength(100)]
        public string? RateCode { get; set; }

        [Column("schedule_from_date", TypeName = "date")]
        public DateTime? ScheduleFromDate { get; set; }

        [Column("schedule_to_date", TypeName = "date")]
        public DateTime? ScheduleToDate { get; set; }

        [Column("charge_code")]
        [StringLength(100)]
        public string? ChargeCode { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("amount_period")]
        public decimal? AmountPeriod { get; set; }

        [Column("estimate_type")]
        public int? EstimateType { get; set; }

        [Column("currency")]
        [StringLength(20)]
        public string? Currency { get; set; }

        [Column("mgmt_fee_code")]
        [StringLength(100)]
        public string? MgmtFeeCode { get; set; }

        [Column("mgmt_fee_percentage")]
        public decimal? MgmtFeePercentage { get; set; }

        [Column("sales_tax_code")]
        [StringLength(100)]
        public string? SalesTaxCode { get; set; }

        [Column("sales_tax_percentage")]
        public decimal? SalesTaxPercentage { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("rent_increase_start_date", TypeName = "date")]
        public DateTime? RentIncreaseStartDate { get; set; }

        [Column("rent_increase_end_date", TypeName = "date")]
        public DateTime? RentIncreaseEndDate { get; set; }

        [Column("invoice_frequency")]
        public int? InvoiceFrequency { get; set; }

        [Column("bill_day")]
        public int? BillDay { get; set; }

        [Column("days_due")]
        public int? DaysDue { get; set; }

        [Column("days_due_after_method")]
        public int? DaysDueAfterMethod { get; set; }

        [Column("unit_code")]
        [StringLength(100)]
        public string? UnitCode { get; set; }

        [Column("area_column_override")]
        public decimal? AreaColumnOverride { get; set; }

        // Billing Flag Months (Jan - Dec)
        [Column("bill_jan")] public int? BillJan { get; set; }
        [Column("bill_feb")] public int? BillFeb { get; set; }
        [Column("bill_mar")] public int? BillMar { get; set; }
        [Column("bill_apr")] public int? BillApr { get; set; }
        [Column("bill_may")] public int? BillMay { get; set; }
        [Column("bill_jun")] public int? BillJun { get; set; }
        [Column("bill_jul")] public int? BillJul { get; set; }
        [Column("bill_aug")] public int? BillAug { get; set; }
        [Column("bill_sep")] public int? BillSep { get; set; }
        [Column("bill_oct")] public int? BillOct { get; set; }
        [Column("bill_nov")] public int? BillNov { get; set; }
        [Column("bill_dec")] public int? BillDec { get; set; }

        [Column("proposal_type")]
        [StringLength(100)]
        public string? ProposalType { get; set; }

        [Column("eft")]
        [StringLength(100)]
        public string? Eft { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("date_last_billed", TypeName = "date")]
        public DateTime? DateLastBilled { get; set; }

        // User-Defined Custom Fields (1 - 20)
        [Column("userdefined_1")] public string? UserDefined1 { get; set; }
        [Column("userdefined_2")] public string? UserDefined2 { get; set; }
        [Column("userdefined_3")] public string? UserDefined3 { get; set; }
        [Column("userdefined_4")] public string? UserDefined4 { get; set; }
        [Column("userdefined_5")] public string? UserDefined5 { get; set; }
        [Column("userdefined_6")] public string? UserDefined6 { get; set; }
        [Column("userdefined_7")] public string? UserDefined7 { get; set; }
        [Column("userdefined_8")] public string? UserDefined8 { get; set; }
        [Column("userdefined_9")] public string? UserDefined9 { get; set; }
        [Column("userdefined_10")] public string? UserDefined10 { get; set; }
        [Column("userdefined_11")] public string? UserDefined11 { get; set; }
        [Column("userdefined_12")] public string? UserDefined12 { get; set; }
        [Column("userdefined_13")] public string? UserDefined13 { get; set; }
        [Column("userdefined_14")] public string? UserDefined14 { get; set; }
        [Column("userdefined_15")] public string? UserDefined15 { get; set; }
        [Column("userdefined_16")] public string? UserDefined16 { get; set; }
        [Column("userdefined_17")] public string? UserDefined17 { get; set; }
        [Column("userdefined_18")] public string? UserDefined18 { get; set; }
        [Column("userdefined_19")] public string? UserDefined19 { get; set; }
        [Column("userdefined_20")] public string? UserDefined20 { get; set; }

        [Column("estimated_rent")]
        public decimal? EstimatedRent { get; set; }

        [Column("review_type")]
        [StringLength(100)]
        public string? ReviewType { get; set; }

        [Column("ninety_day_due_date", TypeName = "date")]
        public DateTime? NinetyDayDueDate { get; set; }

        [Column("ext_schedule_id")]
        [StringLength(100)]
        public string? ExtScheduleId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
