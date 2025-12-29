using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.WorkDate.Date == date.Date);
    }

    public async Task<IEnumerable<Attendance>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId && a.WorkDate.Year == year && a.WorkDate.Month == month)
            .OrderBy(a => a.WorkDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId && a.WorkDate >= fromDate && a.WorkDate <= toDate)
            .OrderBy(a => a.WorkDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Where(a => a.WorkDate.Date == date.Date)
            .ToListAsync();
    }

    public async Task<int> GetWorkingDaysInMonthAsync(int employeeId, int year, int month)
    {
        return await _dbSet
            .Where(a => a.EmployeeId == employeeId 
                && a.WorkDate.Year == year 
                && a.WorkDate.Month == month
                && (a.Status == "Present" || a.Status == "Late"))
            .CountAsync();
    }
}
