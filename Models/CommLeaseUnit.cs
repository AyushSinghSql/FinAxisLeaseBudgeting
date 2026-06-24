using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("comm_lease_units", Schema = "public")]
    public class CommLeaseUnit
    {
        [Key]
        [Column("lease_unit_id")]
        public long LeaseUnitId { get; set; }

        [Required]
        [Column("property_code")]
        [StringLength(100)]
        public string PropertyCode { get; set; } = null!;

        [Required]
        [Column("lease_code")]
        [StringLength(100)]
        public string LeaseCode { get; set; } = null!;

        [Column("unit_code")]
        [StringLength(100)]
        public string? UnitCode { get; set; }

        [Column("ref_property_id")]
        [StringLength(100)]
        public string? RefPropertyId { get; set; }

        [Column("ref_unit_id")]
        [StringLength(100)]
        public string? RefUnitId { get; set; }

        [Column("ref_lease_id")]
        [StringLength(100)]
        public string? RefLeaseId { get; set; }

        [Column("unit_start_date", TypeName = "date")]
        public DateTime? UnitStartDate { get; set; }

        [Column("unit_end_date", TypeName = "date")]
        public DateTime? UnitEndDate { get; set; }

        [Column("unit_move_in_date", TypeName = "date")]
        public DateTime? UnitMoveInDate { get; set; }

        [Column("unit_move_out_date", TypeName = "date")]
        public DateTime? UnitMoveOutDate { get; set; }

        [Column("amendment_type")]
        public int? AmendmentType { get; set; }

        [Column("amendment_sequence")]
        public int? AmendmentSequence { get; set; }

        [Column("parent_amendment_type")]
        public int? ParentAmendmentType { get; set; }

        [Column("parent_amendment_sequence")]
        public int? ParentAmendmentSequence { get; set; }

        [Column("amendment_start_date", TypeName = "date")]
        public DateTime? AmendmentStartDate { get; set; }

        [Column("proposal_type")]
        [StringLength(50)]
        public string? ProposalType { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}