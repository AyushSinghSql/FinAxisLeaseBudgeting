using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinAxisLeaseBudgeting.Models
{
    [Table("chathistories")] 
    public class ChatHistoryMessage
    {
        [Key]
        [Column("Id")] 
        public int Id { get; set; }

        [Required]
        [Column("UserId")] 
        public int UserId { get; set; }

        [Required]
        [Column("SessionId")]
        public string SessionId { get; set; } = string.Empty;

        [Required]
        [Column("UserQuery")]
        public string UserQuery { get; set; } = string.Empty;

        [Required]
        [Column("QueryHash")]
        public string QueryHash { get; set; } = string.Empty;

        [Required]
        [Column("AssistantResponse")]
        public string AssistantResponse { get; set; } = string.Empty;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}