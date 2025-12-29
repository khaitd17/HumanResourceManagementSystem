using HRM.DataLayer.Entities;

namespace HRM.RepositoryLayer.Interfaces;

public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId);
    Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync();
    Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status);
    Task<IEnumerable<LeaveRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime fromDate, DateTime toDate, int? excludeId = null);
}
