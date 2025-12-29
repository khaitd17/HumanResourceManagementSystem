using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IChatRoomRepository : IGenericRepository<ChatRoom>
{
    Task<IEnumerable<ChatRoom>> GetByEmployeeAsync(int employeeId);
    Task<ChatRoom?> GetDirectChatRoomAsync(int employee1Id, int employee2Id);
    Task<ChatRoom?> GetWithMembersAsync(int chatRoomId);
}
