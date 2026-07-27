using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlanningAPI.Models
{

    //public class UserAccess
    //{
    //    public int UserId { get; set; }
    //    public string Username { get; set; } = null!;
    //    public bool IsActive { get; set; } = true;

    //    public int RoleId { get; set; }
    //    public Role Role { get; set; } = null!;

    //    public ICollection<UserScreenPermission> ScreenOverrides { get; set; } = new List<UserScreenPermission>();
    //    public ICollection<UserFieldPermission> FieldOverrides { get; set; } = new List<UserFieldPermission>();
    //}

    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [Column("role_name")]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;
        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<RoleScreenPermission> ScreenPermissions { get; set; } = new List<RoleScreenPermission>();

        public ICollection<RoleFieldPermission> FieldPermissions { get; set; } = new List<RoleFieldPermission>();
    }


    [Table("role_screen_permissions")]
    public class RoleScreenPermission
    {
        [Column("role_id")]
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        [Column("screen_code")]
        [StringLength(100)]
        public string ScreenCode { get; set; } = null!;

        [Column("can_view")]
        public bool CanView { get; set; }

        [Column("can_edit")]
        public bool CanEdit { get; set; }

        public Role Role { get; set; } = null!;
    }
    [Table("role_field_permissions")]
    public class RoleFieldPermission
    {
        [Column("role_id")]
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        [Column("field_code")]
        [StringLength(100)]
        public string FieldCode { get; set; } = null!;

        [Column("can_view")]
        public bool CanView { get; set; }

        [Column("can_edit")]
        public bool CanEdit { get; set; }

        public Role Role { get; set; } = null!;
    }

    [Table("user_screen_permissions")]
    public class UserScreenPermission
    {
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Column("screen_code")]
        [StringLength(100)]
        public string ScreenCode { get; set; } = null!;

        [Column("can_view")]
        public bool CanView { get; set; }

        [Column("can_edit")]
        public bool CanEdit { get; set; }

        public User User { get; set; } = null!;
    }

    [Table("user_field_permissions")]
    public class UserFieldPermission
    {
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Column("field_code")]
        [StringLength(100)]
        public string FieldCode { get; set; } = null!;

        [Column("can_view")]
        public bool CanView { get; set; }

        [Column("can_edit")]
        public bool CanEdit { get; set; }

        public User User { get; set; } = null!;
    }
    public class PermissionResponse
    {
        public Dictionary<string, PermissionAction> Screens { get; set; } = new();
        public Dictionary<string, PermissionAction> Fields { get; set; } = new();
    }

    public class PermissionAction
    {
        public bool View { get; set; }
        public bool Edit { get; set; }
    }
    public class UserSettingsRequest
    {
        public int UserId { get; set; }

        public Dictionary<string, PermissionAction>? Screens { get; set; }
        public Dictionary<string, PermissionAction>? Fields { get; set; }
    }
    public class RoleSettingsRequest
    {
        public int RoleId { get; set; }

        public Dictionary<string, PermissionAction>? Screens { get; set; }
        public Dictionary<string, PermissionAction>? Fields { get; set; }
    }
    public class RolePermissionsResponse
    {
        public int RoleId { get; set; }
        public Dictionary<string, PermissionAction> Screens { get; set; } = new();
        public Dictionary<string, PermissionAction> Fields { get; set; } = new();
    }
    public class BulkRolePermissionRequest
    {
        public int RoleId { get; set; }

        public List<BulkScreenPermission>? Screens { get; set; }
        public List<BulkFieldPermission>? Fields { get; set; }
    }

    public class BulkScreenPermission
    {
        public string ScreenCode { get; set; } = null!;
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    public class BulkFieldPermission
    {
        public string FieldCode { get; set; } = null!;
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
    public class BulkUserPermissionRequest
    {
        public int UserId { get; set; }

        public List<BulkScreenPermission>? Screens { get; set; }
        public List<BulkFieldPermission>? Fields { get; set; }
    }

    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }




[Table("reportgroups")]
    public class ReportGroup
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ReportGroupReportMapping> Reports { get; set; }
            = new List<ReportGroupReportMapping>();
    }

    [Table("reportgroup_report_mapping")]
    public class ReportGroupReportMapping
    {
        [Column("reportgroup_id")]
        [ForeignKey(nameof(ReportGroup))]
        public int ReportGroupId { get; set; }

        [Column("report_code")]
        public string ReportCode { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ReportGroup ReportGroup { get; set; } = null!;
    }
    public class ReportGroupDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Reports { get; set; } = new();
    }

    public class SaveReportGroupDto
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Reports { get; set; } = new();
    }

}
