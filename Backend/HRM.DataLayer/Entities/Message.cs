using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("Messages")]
public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ChatRoomId { get; set; }

    public int SenderId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AttachmentUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }

    // Navigation Properties
    [ForeignKey("ChatRoomId")]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    [ForeignKey("SenderId")]
    public virtual Employee Sender { get; set; } = null!;
}
