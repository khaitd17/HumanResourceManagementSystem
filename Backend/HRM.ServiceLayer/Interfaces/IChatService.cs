using HRM.ServiceLayer.DTOs.Chat;
using HRM.ServiceLayer.DTOs.Common;

namespace HRM.ServiceLayer.Interfaces;

public interface IChatService
{
    Task<ServiceResult<ChatRoomDto>> GetChatRoomByIdAsync(int chatRoomId);
    Task<ServiceResult<List<ChatRoomDto>>> GetChatRoomsByEmployeeAsync(int employeeId);
    Task<ServiceResult<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto dto);
    Task<ServiceResult<ChatRoomDto>> GetOrCreateDirectChatAsync(int employee1Id, int employee2Id);
    Task<ServiceResult<List<MessageDto>>> GetMessagesAsync(int chatRoomId, int pageNumber = 1, int pageSize = 50);
    Task<ServiceResult<MessageDto>> SendMessageAsync(SendMessageDto dto);
    Task<ServiceResult> MarkAsReadAsync(int messageId);
    Task<ServiceResult<int>> GetUnreadCountAsync(int chatRoomId, int employeeId);
}
