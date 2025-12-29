namespace HRM.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    IAttendanceRepository Attendances { get; }
    ILeaveRequestRepository LeaveRequests { get; }
    IPayrollRepository Payrolls { get; }
    IPayrollConfigRepository PayrollConfigs { get; }
    IChatRoomRepository ChatRooms { get; }
    IChatRoomMemberRepository ChatRoomMembers { get; }
    IMessageRepository Messages { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
