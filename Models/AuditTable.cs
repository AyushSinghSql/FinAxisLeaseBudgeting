using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanningAPI.Models

{

    [Table("plan_audit_log")]

    public class AuditTable
    {

        [Key]

        [Column("audit_id")]

        public long AuditId { get; set; }


        [Required]

        [Column("project_id")]

        [StringLength(100)]

        public string ProjectId { get; set; } = string.Empty;


        [Required]

        [Column("plan_id")]

        public int PlanId { get; set; }


        [Column("plan_type")]

        [StringLength(20)]

        public string? PlanType { get; set; }


        [Required]

        [Column("username")]

        [StringLength(100)]

        public string Username { get; set; } = string.Empty;


        [Required]

        [Column("action_type")]

        [StringLength(20)]

        public string ActionType { get; set; } = string.Empty;


        [Column("action_details")]

        public string? ActionDetails { get; set; }


        [Column("version")]

        public long Version { get; set; }


        [Column("created_at")]

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}