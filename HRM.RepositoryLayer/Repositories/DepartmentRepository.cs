using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Name == name);
    }

    public async Task<IEnumerable<Department>> GetWithEmployeesAsync()
    {
        return await _dbSet
            .Include(d => d.Employees.Where(e => e.IsActive))
            .ToListAsync();
    }
}
