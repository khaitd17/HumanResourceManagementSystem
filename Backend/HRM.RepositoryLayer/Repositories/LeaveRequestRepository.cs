using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
            .Where(lr => lr.EmployeeId == employeeId && lr.DeletedAt == null)
            .OrderByDescending(lr => lr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync()
    {
        return await _dbSet
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == "Pending" && lr.DeletedAt == null)
            .OrderBy(lr => lr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == status && lr.DeletedAt == null)
            .OrderByDescending(lr => lr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
            .Where(lr => lr.FromDate <= toDate && lr.ToDate >= fromDate && lr.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime fromDate, DateTime toDate, int? excludeId = null)
    {
        var query = _dbSet.Where(lr => 
            lr.EmployeeId == employeeId 
            && lr.Status == "Approved"
            && lr.DeletedAt == null
            && lr.FromDate <= toDate 
            && lr.ToDate >= fromDate);

        if (excludeId.HasValue)
        {
            query = query.Where(lr => lr.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
