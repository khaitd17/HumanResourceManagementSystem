using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("ChatRooms")]
public class ChatRoom
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Type { get; set; } = "Group"; // Direct, Group

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
