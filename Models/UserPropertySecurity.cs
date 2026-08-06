namespace FinAxisLeaseBudgeting.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("user_property_security")]
    public class UserPropertySecurity
    {
        [Key]
        [Column("user_property_security_id")]
        public long UserPropertySecurityId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("entity_id")]
        [StringLength(50)]
        public string? EntityId { get; set; }

        [Column("property_id")]
        [StringLength(50)]
        public string PropertyId { get; set; } = string.Empty;

        [Column("access_level")]
        [StringLength(20)]
        public string AccessLevel { get; set; } = "READ";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("created_by")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }

    public class UserPropertySecurityRequest
    {
        public long UserId { get; set; }

        public List<PropertySecurityDto> Properties { get; set; } = new();
    }

    public class PropertySecurityDto
    {
        public string PropertyId { get; set; } = string.Empty;

        public string? EntityId { get; set; }

        public string AccessLevel { get; set; } = "READ";
    }
}
