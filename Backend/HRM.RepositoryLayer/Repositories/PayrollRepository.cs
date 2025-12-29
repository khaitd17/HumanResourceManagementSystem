using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class PayrollRepository : GenericRepository<Payroll>, IPayrollRepository
{
    public PayrollRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<Payroll?> GetByEmployeeAndMonthAsync(int employeeId, int year, int month)
    {
        return await _dbSet
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month && p.DeletedAt == null);
    }

    public async Task<IEnumerable<Payroll>> GetByMonthAsync(int year, int month)
    {
        return await _dbSet
            .Include(p => p.Employee)
                .ThenInclude(e => e.Department)
            .Where(p => p.Year == year && p.Month == month && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payroll>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbSet
            .Include(p => p.Employee)
            .Where(p => p.EmployeeId == employeeId && p.DeletedAt == null)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payroll>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(p => p.Employee)
            .Where(p => p.Status == status && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<bool> PayrollExistsAsync(int employeeId, int year, int month)
    {
        return await _dbSet.AnyAsync(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month && p.DeletedAt == null);
    }
}
