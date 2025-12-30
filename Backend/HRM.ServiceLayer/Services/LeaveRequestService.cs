using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.LeaveRequest;
using HRM.ServiceLayer.Interfaces;

namespace HRM.ServiceLayer.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeaveRequestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<LeaveRequestDto>> GetByIdAsync(int id)
    {
        try
        {
            var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult("Leave request not found");
            }

            var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return ServiceResult<LeaveRequestDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<LeaveRequestDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<LeaveRequestDto>>> GetByEmployeeAsync(int employeeId)
    {
        try
        {
            var leaveRequests = await _unitOfWork.LeaveRequests.GetByEmployeeAsync(employeeId);
            var dtos = _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
            return ServiceResult<List<LeaveRequestDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<LeaveRequestDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<LeaveRequestDto>>> GetPendingRequestsAsync()
    {
        try
        {
            var leaveRequests = await _unitOfWork.LeaveRequests.GetPendingRequestsAsync();
            var dtos = _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
            return ServiceResult<List<LeaveRequestDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<LeaveRequestDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<LeaveRequestDto>>> GetByStatusAsync(string status)
    {
        try
        {
            var leaveRequests = await _unitOfWork.LeaveRequests.GetByStatusAsync(status);
            var dtos = _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
            return ServiceResult<List<LeaveRequestDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<LeaveRequestDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto)
    {
        try
        {
            // Validate employee exists
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult("Employee not found");
            }

            // Check if employee is active
            if (!employee.IsActive)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult("Employee is not active");
            }

            // Validate dates
            if (dto.FromDate > dto.ToDate)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult("From date must be before or equal to date");
            }

            // Check for overlapping approved leaves
            var hasOverlap = await _unitOfWork.LeaveRequests.HasOverlappingLeaveAsync(
                dto.EmployeeId, 
                dto.FromDate, 
                dto.ToDate
            );

            if (hasOverlap)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult(
                    "You already have an approved leave request for this period"
                );
            }

            var leaveRequest = _mapper.Map<LeaveRequest>(dto);
            
            await _unitOfWork.LeaveRequests.AddAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return ServiceResult<LeaveRequestDto>.SuccessResult(result, "Leave request submitted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<LeaveRequestDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<LeaveRequestDto>> ApproveAsync(ApproveLeaveRequestDto dto)
    {
        try
        {
            var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(dto.LeaveRequestId);
            if (leaveRequest == null)
            {
                return ServiceResult<LeaveRequestDto>.FailureResult("Leave request not found");
            }

            if (leaveRequest.Status != "Pending")
            {
                return ServiceResult<LeaveRequestDto>.FailureResult(
                    $"Leave request is already {leaveRequest.Status.ToLower()}"
                );
            }

            // Update status
            leaveRequest.Status = dto.IsApproved ? "Approved" : "Rejected";
            leaveRequest.ApprovedBy = dto.ApprovedBy;
            leaveRequest.ApprovedAt = DateTime.UtcNow;

            if (!dto.IsApproved && !string.IsNullOrEmpty(dto.RejectionReason))
            {
                leaveRequest.RejectionReason = dto.RejectionReason;
            }

            _unitOfWork.LeaveRequests.Update(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Send notification to employee via SignalR

            var result = _mapper.Map<LeaveRequestDto>(leaveRequest);
            var message = dto.IsApproved 
                ? "Leave request approved successfully" 
                : "Leave request rejected";

            return ServiceResult<LeaveRequestDto>.SuccessResult(result, message);
        }
        catch (Exception ex)
        {
            return ServiceResult<LeaveRequestDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null)
            {
                return ServiceResult.FailureResult("Leave request not found");
            }

            // Only allow deletion of pending requests
            if (leaveRequest.Status != "Pending")
            {
                return ServiceResult.FailureResult("Only pending leave requests can be deleted");
            }

            // Soft delete
            leaveRequest.DeletedAt = DateTime.UtcNow;
            _unitOfWork.LeaveRequests.Update(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Leave request deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }
}
