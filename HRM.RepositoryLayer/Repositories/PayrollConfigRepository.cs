using HRM.DataLayer.Data;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.RepositoryLayer.Repositories;

public class PayrollConfigRepository : GenericRepository<PayrollConfig>, IPayrollConfigRepository
{
    public PayrollConfigRepository(HRMDbContext context) : base(context)
    {
    }

    public async Task<PayrollConfig?> GetActiveConfigAsync()
    {
        return await _dbSet
            .Where(pc => pc.IsActive)
            .OrderByDescending(pc => pc.EffectiveFrom)
            .FirstOrDefaultAsync();
    }

    public async Task<PayrollConfig?> GetConfigByDateAsync(DateTime date)
    {
        return await _dbSet
            .Where(pc => pc.EffectiveFrom <= date && (pc.EffectiveTo == null || pc.EffectiveTo >= date))
            .OrderByDescending(pc => pc.EffectiveFrom)
            .FirstOrDefaultAsync();
    }
}
