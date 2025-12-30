namespace HRM.ServiceLayer.DTOs.LeaveRequest;

public class CreateLeaveRequestDto
{
    public int EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string LeaveType { get; set; } = "Annual";
}
