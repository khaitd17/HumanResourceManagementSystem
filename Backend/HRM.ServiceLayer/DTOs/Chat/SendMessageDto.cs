namespace HRM.ServiceLayer.DTOs.Chat;

public class SendMessageDto
{
    public int ChatRoomId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}
