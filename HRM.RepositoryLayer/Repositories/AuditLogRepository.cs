using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(int userId)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByTableAsync(string tableName)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.TableName == tableName)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.CreatedAt >= fromDate && al.CreatedAt <= toDate)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();
    }
}
