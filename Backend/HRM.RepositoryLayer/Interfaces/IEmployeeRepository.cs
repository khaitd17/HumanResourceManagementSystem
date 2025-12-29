using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByEmployeeCodeAsync(string employeeCode);
    Task<Employee?> GetByEmailAsync(string email);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<bool> EmployeeCodeExistsAsync(string employeeCode);
    Task<bool> EmailExistsAsync(string email);
}
