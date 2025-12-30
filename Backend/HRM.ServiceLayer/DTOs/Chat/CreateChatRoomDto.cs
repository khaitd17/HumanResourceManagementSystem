namespace HRM.ServiceLayer.DTOs.Chat;

public class CreateChatRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Group"; // Direct or Group
    public List<int> MemberIds { get; set; } = new();
}
