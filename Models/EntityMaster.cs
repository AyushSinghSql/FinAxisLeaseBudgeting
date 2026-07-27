using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanningAPI.Models
{
    [Table("entity_master")]
    public class EntityMaster
    {
        [Key]
        [Column("entity_id")]
        [StringLength(50)]
        public string EntityId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("entity_code")]
        [StringLength(50)]
        public string EntityCode { get; set; } = string.Empty;

        [Required]
        [Column("entity_name")]
        [StringLength(200)]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        [Column("base_currency")]
        [StringLength(10)]
        public string BaseCurrency { get; set; } = string.Empty;

        [Column("country")]
        [StringLength(100)]
        public string? Country { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("parent_entity_id")]
        [StringLength(50)]
        public string? ParentEntityId { get; set; }

        [ForeignKey(nameof(ParentEntityId))]
        public EntityMaster? ParentEntity { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Column("region")]
        [StringLength(10)]
        public string? Region { get; set; }

        [Column("ownership_group")]
        [StringLength(30)]
        public string? OwnershipGroup { get; set; }
    }
}