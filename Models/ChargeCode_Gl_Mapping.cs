using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("charge_cd_gl_account")]
    public class ChargeCdGlAccount
    {
        [Key]
        [Column("charge_code", Order = 0)]
        [StringLength(50)]
        public string ChargeCode { get; set; } = string.Empty;

        [Key]
        [Column("gl_account", Order = 1)]
        [StringLength(50)]
        public string GlAccount { get; set; } = string.Empty;

        [Key]
        [Column("revenue_type", Order = 2)]
        [StringLength(20)]
        public string RevenueType { get; set; } = string.Empty;

        [Column("charge_description")]
        [StringLength(250)]
        public string? ChargeDescription { get; set; }

        [Column("gl_account_name")]
        [StringLength(250)]
        public string? GlAccountName { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }

    public class ChargeAccountDto
    {
        public string ChargeCode { get; set; } = string.Empty;

        public string ChargeDescription { get; set; } = string.Empty;

        public string AccountId { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;
    }
}
