using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IPayrollConfigRepository : IGenericRepository<PayrollConfig>
{
    Task<PayrollConfig?> GetActiveConfigAsync();
    Task<PayrollConfig?> GetConfigByDateAsync(DateTime date);
}
