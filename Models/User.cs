using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace PlanningAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [Column("full_name")]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Column("email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Column("password_hash")]
        [StringLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("role_id")]
        [ForeignKey(nameof(UserRole))]
        public int RoleId { get; set; }

        public virtual Role UserRole { get; set; } = null!;

        public virtual ICollection<UserScreenPermission> ScreenOverrides { get; set; } = new List<UserScreenPermission>();

        public virtual ICollection<UserFieldPermission> FieldOverrides { get; set; } = new List<UserFieldPermission>();
    }
    //public class UserProjectMap
    //{
    //    public int UserId { get; set; }
    //    public string ProjId { get; set; } = null!;
    //    public DateTime AssignedAt { get; set; }

    //    public User User { get; set; } = null!;
    //    public PlProject Project { get; set; } = null!;
    //}

    //public class UserGroupMap
    //{
    //    public int UserId { get; set; }
    //    public string ProjId { get; set; } = null!;
    //    public DateTime AssignedAt { get; set; }

    //    public User User { get; set; } = null!;
    //    public OrgGroup OrgGroup { get; set; } = null!;
    //}

    //public class OrgGroupUserMapping
    //{
    //    public int OrgGroupId { get; set; }
    //    public int UserId { get; set; }

    //    public bool IsActive { get; set; } = true;
    //    public DateTime? AssignedAt { get; set; }
    //    public string? AssignedBy { get; set; }

    //    //public OrgGroup OrgGroup { get; set; }
    //    public User User { get; set; }
    //}

    //[Table("user_org_mapping", Schema = "public")]
    //public class UserOrgMapping
    //{
    //    public string OrgId { get; set; }
    //    public int UserId { get; set; }
    //    public PlOrgnization Orgnization { get; set; }
    //    public User User { get; set; }
    //}



    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class UserConfiguration
    {
        public int UserId { get; set; }
        public Visibility Visibility { get; set; }
        public string configType { get; set; }
    }

    public class Visibility
    {
        public bool projectHours { get; set; }

        [JsonProperty("projectHours.idType")]
        public bool projectHoursidType { get; set; }

        [JsonProperty("projectHours.emplId")]
        public bool projectHoursemplId { get; set; }

        [JsonProperty("projectHours.warning")]
        public bool projectHourswarning { get; set; }

        [JsonProperty("projectHours.name")]
        public bool projectHoursname { get; set; }

        [JsonProperty("projectHours.acctId")]
        public bool projectHoursacctId { get; set; }

        [JsonProperty("projectHours.acctName")]
        public bool projectHoursacctName { get; set; }

        [JsonProperty("projectHours.orgId")]
        public bool projectHoursorgId { get; set; }

        [JsonProperty("projectHours.glcPlc")]
        public bool projectHoursglcPlc { get; set; }

        [JsonProperty("projectHours.isRev")]
        public bool projectHoursisRev { get; set; }

        [JsonProperty("projectHours.isBrd")]
        public bool projectHoursisBrd { get; set; }

        [JsonProperty("projectHours.status")]
        public bool projectHoursstatus { get; set; }

        [JsonProperty("projectHours.perHourRate")]
        public bool projectHoursperHourRate { get; set; }

        [JsonProperty("projectHours.total")]
        public bool projectHourstotal { get; set; }
        public bool projectAmounts { get; set; }

        [JsonProperty("projectAmounts.idType")]
        public bool projectAmountsidType { get; set; }

        [JsonProperty("projectAmounts.emplId")]
        public bool projectAmountsemplId { get; set; }

        [JsonProperty("projectAmounts.name")]
        public bool projectAmountsname { get; set; }

        [JsonProperty("projectAmounts.acctId")]
        public bool projectAmountsacctId { get; set; }

        [JsonProperty("projectAmounts.acctName")]
        public bool projectAmountsacctName { get; set; }

        [JsonProperty("projectAmounts.orgId")]
        public bool projectAmountsorgId { get; set; }

        [JsonProperty("projectAmounts.isRev")]
        public bool projectAmountsisRev { get; set; }

        [JsonProperty("projectAmounts.isBrd")]
        public bool projectAmountsisBrd { get; set; }

        [JsonProperty("projectAmounts.status")]
        public bool projectAmountsstatus { get; set; }

        [JsonProperty("projectAmounts.total")]
        public bool projectAmountstotal { get; set; }
    }


    [Table("user_login_audit")]

    public class UserLoginHistory
    {

        [Key]

        [Column("id")]

        public int Id { get; set; }


        [Column("user_id")]

        public int UserId { get; set; }


        [Column("login_time")]

        public DateTime LoginTime { get; set; }


        [Column("ip_address")]

        public string? IpAddress { get; set; }

    }

}
