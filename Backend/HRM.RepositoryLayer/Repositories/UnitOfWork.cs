using HRM.DataLayer.Data;
using HRM.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRM.RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HRMDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; }
    public IEmployeeRepository Employees { get; }
    public IDepartmentRepository Departments { get; }
    public IAttendanceRepository Attendances { get; }
    public ILeaveRequestRepository LeaveRequests { get; }
    public IPayrollRepository Payrolls { get; }
    public IPayrollConfigRepository PayrollConfigs { get; }
    public IChatRoomRepository ChatRooms { get; }
    public IChatRoomMemberRepository ChatRoomMembers { get; }
    public IMessageRepository Messages { get; }
    public IAuditLogRepository AuditLogs { get; }

    public UnitOfWork(HRMDbContext context)
    {
        _context = context;

        // Initialize all repositories
        Users = new UserRepository(_context);
        Employees = new EmployeeRepository(_context);
        Departments = new DepartmentRepository(_context);
        Attendances = new AttendanceRepository(_context);
        LeaveRequests = new LeaveRequestRepository(_context);
        Payrolls = new PayrollRepository(_context);
        PayrollConfigs = new PayrollConfigRepository(_context);
        ChatRooms = new ChatRoomRepository(_context);
        ChatRoomMembers = new ChatRoomMemberRepository(_context);
        Messages = new MessageRepository(_context);
        AuditLogs = new AuditLogRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
