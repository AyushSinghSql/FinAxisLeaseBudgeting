using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("property_master")]
    public class PropertyMaster
    {
        [Key]
        [Column("property_id")]
        public string PropertyId { get; set; } = string.Empty;

        [Column("property_code")]
        public string PropertyCode { get; set; } = string.Empty;

        [Column("property_name")]
        public string PropertyName { get; set; } = string.Empty;

        [Column("entity_id")]
        public string EntityId { get; set; } = string.Empty;

        [Column("property_type")]
        public string? PropertyType { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("state")]
        public string? State { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        [Column("currency")]
        public string? Currency { get; set; }

        [Column("area")]
        public decimal? Area { get; set; }

        [Column("gla")]
        public decimal? Gla { get; set; }

        [Column("nla")]
        public decimal? Nla { get; set; }

        [Column("acquisition_date")]
        public DateTime? AcquisitionDate { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
    }

    public class PropertyDropdownDto
    {
        public string PropertyId { get; set; } = string.Empty;
        public string PropertyCode { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
    }
}