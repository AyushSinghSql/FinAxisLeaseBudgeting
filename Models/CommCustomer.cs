using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("comm_customers", Schema = "public")]
    public class CommCustomer
    {
        [Key]
        [Column("customer_id")]
        public long CustomerId { get; set; }

        [Column("customer_code")]
        [StringLength(100)]
        public string? CustomerCode { get; set; }

        [Column("customer_name")]
        [StringLength(255)]
        public string? CustomerName { get; set; }

        [Column("ext_ref_customer_id")]
        [StringLength(100)]
        public string? ExtRefCustomerId { get; set; }

        [Column("customer_type")]
        [StringLength(100)]
        public string? CustomerType { get; set; }

        [Column("customer_status")]
        [StringLength(100)]
        public string? CustomerStatus { get; set; }

        [Column("ext_ref_customer_parent_id")]
        [StringLength(100)]
        public string? ExtRefCustomerParentId { get; set; }

        [Column("customer_parent")]
        [StringLength(255)]
        public string? CustomerParent { get; set; }

        [Column("preferred_language")]
        [StringLength(100)]
        public string? PreferredLanguage { get; set; }

        [Column("sales_category")]
        [StringLength(100)]
        public string? SalesCategory { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // User-Defined Custom Fields (1 - 6)
        [Column("userdefined_1")] public string? UserDefined1 { get; set; }
        [Column("userdefined_2")] public string? UserDefined2 { get; set; }
        [Column("userdefined_3")] public string? UserDefined3 { get; set; }
        [Column("userdefined_4")] public string? UserDefined4 { get; set; }
        [Column("userdefined_5")] public string? UserDefined5 { get; set; }
        [Column("userdefined_6")] public string? UserDefined6 { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}