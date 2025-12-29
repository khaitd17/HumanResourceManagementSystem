using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IPayrollRepository : IGenericRepository<Payroll>
{
    Task<Payroll?> GetByEmployeeAndMonthAsync(int employeeId, int year, int month);
    Task<IEnumerable<Payroll>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<Payroll>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<Payroll>> GetByStatusAsync(string status);
    Task<bool> PayrollExistsAsync(int employeeId, int year, int month);
}
