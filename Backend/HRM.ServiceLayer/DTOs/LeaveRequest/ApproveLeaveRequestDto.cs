namespace HRM.ServiceLayer.DTOs.LeaveRequest;

public class ApproveLeaveRequestDto
{
    public int LeaveRequestId { get; set; }
    public int ApprovedBy { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
}
