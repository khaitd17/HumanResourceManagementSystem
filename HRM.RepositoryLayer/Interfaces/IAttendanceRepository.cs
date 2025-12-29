using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface IAttendanceRepository : IGenericRepository<Attendance>
{
    Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
    Task<IEnumerable<Attendance>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month);
    Task<IEnumerable<Attendance>> GetByDateRangeAsync(int employeeId, DateTime fromDate, DateTime toDate);
    Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);
    Task<int> GetWorkingDaysInMonthAsync(int employeeId, int year, int month);
}
