using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetByChatRoomAsync(int chatRoomId, int pageNumber = 1, int pageSize = 50)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(int chatRoomId, int employeeId)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Where(m => m.ChatRoomId == chatRoomId && m.SenderId != employeeId && !m.IsRead)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int chatRoomId, int employeeId)
    {
        return await _dbSet
            .Where(m => m.ChatRoomId == chatRoomId && m.SenderId != employeeId && !m.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int messageId)
    {
        var message = await _dbSet.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
        }
    }
}
