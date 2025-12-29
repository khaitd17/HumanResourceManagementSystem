using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class ChatRoomMemberRepository : GenericRepository<ChatRoomMember>, IChatRoomMemberRepository
{
    public ChatRoomMemberRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChatRoomMember>> GetByChatRoomAsync(int chatRoomId)
    {
        return await _dbSet
            .Include(crm => crm.Employee)
            .Where(crm => crm.ChatRoomId == chatRoomId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatRoomMember>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(crm => crm.ChatRoom)
            .Where(crm => crm.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<bool> IsMemberAsync(int chatRoomId, int employeeId)
    {
        return await _dbSet.AnyAsync(crm => crm.ChatRoomId == chatRoomId && crm.EmployeeId == employeeId);
    }
}
