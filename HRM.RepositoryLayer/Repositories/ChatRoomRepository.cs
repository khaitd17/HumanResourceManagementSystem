using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
{
    public ChatRoomRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChatRoom>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(cr => cr.ChatRoomMembers)
                .ThenInclude(crm => crm.Employee)
            .Where(cr => cr.ChatRoomMembers.Any(crm => crm.EmployeeId == employeeId))
            .ToListAsync();
    }

    public async Task<ChatRoom?> GetDirectChatRoomAsync(int employee1Id, int employee2Id)
    {
        return await _dbSet
            .Include(cr => cr.ChatRoomMembers)
            .Where(cr => cr.Type == "Direct" 
                && cr.ChatRoomMembers.Count == 2
                && cr.ChatRoomMembers.Any(crm => crm.EmployeeId == employee1Id)
                && cr.ChatRoomMembers.Any(crm => crm.EmployeeId == employee2Id))
            .FirstOrDefaultAsync();
    }

    public async Task<ChatRoom?> GetWithMembersAsync(int chatRoomId)
    {
        return await _dbSet
            .Include(cr => cr.ChatRoomMembers)
                .ThenInclude(crm => crm.Employee)
            .FirstOrDefaultAsync(cr => cr.Id == chatRoomId);
    }
}
