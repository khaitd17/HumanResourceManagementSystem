using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IChatRoomMemberRepository : IGenericRepository<ChatRoomMember>
{
    Task<IEnumerable<ChatRoomMember>> GetByChatRoomAsync(int chatRoomId);
    Task<IEnumerable<ChatRoomMember>> GetByEmployeeAsync(int employeeId);
    Task<bool> IsMemberAsync(int chatRoomId, int employeeId);
}
