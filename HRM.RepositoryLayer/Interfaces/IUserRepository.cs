using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmployeeIdAsync(int employeeId);
    Task<bool> UsernameExistsAsync(string username);
}
