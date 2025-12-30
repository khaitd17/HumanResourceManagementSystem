using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.LeaveRequest;

namespace HRM.ServiceLayer.Interfaces;

public interface ILeaveRequestService
{
    Task<ServiceResult<LeaveRequestDto>> GetByIdAsync(int id);
    Task<ServiceResult<List<LeaveRequestDto>>> GetByEmployeeAsync(int employeeId);
    Task<ServiceResult<List<LeaveRequestDto>>> GetPendingRequestsAsync();
    Task<ServiceResult<List<LeaveRequestDto>>> GetByStatusAsync(string status);
    Task<ServiceResult<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto);
    Task<ServiceResult<LeaveRequestDto>> ApproveAsync(ApproveLeaveRequestDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
