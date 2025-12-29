using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("ChatRoomMembers")]
public class ChatRoomMember
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ChatRoomId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("ChatRoomId")]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
