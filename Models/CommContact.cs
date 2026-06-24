using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("comm_contacts", Schema = "public")]
    public class CommContact
    {
        [Key]
        [Column("contact_id")]
        public long ContactId { get; set; }

        [Column("contact_type_association")]
        [StringLength(100)]
        public string? ContactTypeAssociation { get; set; }

        [Column("record_code")]
        [StringLength(100)]
        public string? RecordCode { get; set; }

        [Column("role_description")]
        [StringLength(255)]
        public string? RoleDescription { get; set; }

        [Column("last_name")]
        [StringLength(255)]
        public string? LastName { get; set; }

        [Column("ext_ref_id")]
        [StringLength(100)]
        public string? ExtRefId { get; set; }

        [Column("ref_record_id")]
        [StringLength(100)]
        public string? RefRecordId { get; set; }

        [Column("is_primary")]
        public bool? IsPrimary { get; set; }

        [Column("detach_contact")]
        public bool? DetachContact { get; set; }

        [Column("contact_code")]
        [StringLength(100)]
        public string? ContactCode { get; set; }

        [Column("first_name")]
        [StringLength(255)]
        public string? FirstName { get; set; }

        [Column("salutation")]
        [StringLength(50)]
        public string? Salutation { get; set; }

        [Column("salutation_2")]
        [StringLength(50)]
        public string? Salutation2 { get; set; }

        [Column("company_name")]
        [StringLength(255)]
        public string? CompanyName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("title")]
        [StringLength(255)]
        public string? Title { get; set; }

        [Column("url")]
        [StringLength(500)]
        public string? Url { get; set; }

        [Column("contact_date", TypeName = "date")]
        public DateTime? ContactDate { get; set; }

        [Column("address_1")]
        [StringLength(255)]
        public string? Address1 { get; set; }

        [Column("address_2")]
        [StringLength(255)]
        public string? Address2 { get; set; }

        [Column("address_3")]
        [StringLength(255)]
        public string? Address3 { get; set; }

        [Column("address_4")]
        [StringLength(255)]
        public string? Address4 { get; set; }

        [Column("city")]
        [StringLength(100)]
        public string? City { get; set; }

        [Column("state")]
        [StringLength(100)]
        public string? State { get; set; }

        [Column("zip_code")]
        [StringLength(20)]
        public string? ZipCode { get; set; }

        [Column("country")]
        [StringLength(100)]
        public string? Country { get; set; }

        // Phone Numbers 1 through 10
        [Column("phone_number_1")][StringLength(50)] public string? PhoneNumber1 { get; set; }
        [Column("phone_number_2")][StringLength(50)] public string? PhoneNumber2 { get; set; }
        [Column("phone_number_3")][StringLength(50)] public string? PhoneNumber3 { get; set; }
        [Column("phone_number_4")][StringLength(50)] public string? PhoneNumber4 { get; set; }
        [Column("phone_number_5")][StringLength(50)] public string? PhoneNumber5 { get; set; }
        [Column("phone_number_6")][StringLength(50)] public string? PhoneNumber6 { get; set; }
        [Column("phone_number_7")][StringLength(50)] public string? PhoneNumber7 { get; set; }
        [Column("phone_number_8")][StringLength(50)] public string? PhoneNumber8 { get; set; }
        [Column("phone_number_9")][StringLength(50)] public string? PhoneNumber9 { get; set; }
        [Column("phone_number_10")][StringLength(50)] public string? PhoneNumber10 { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string? Email { get; set; }

        [Column("alternate_email")]
        [StringLength(255)]
        public string? AlternateEmail { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("contact_owner")]
        [StringLength(255)]
        public string? ContactOwner { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}