using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetByNameAsync(string name);
    Task<IEnumerable<Department>> GetWithEmployeesAsync();
}
