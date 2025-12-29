using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetByChatRoomAsync(int chatRoomId, int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(int chatRoomId, int employeeId);
    Task<int> GetUnreadCountAsync(int chatRoomId, int employeeId);
    Task MarkAsReadAsync(int messageId);
}
