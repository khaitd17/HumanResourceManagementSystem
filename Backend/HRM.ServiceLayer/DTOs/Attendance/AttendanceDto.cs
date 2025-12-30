namespace HRM.ServiceLayer.DTOs.Attendance;

public class AttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime WorkDate { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public decimal TotalHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? LateMinutes { get; set; }
    public int? EarlyLeaveMinutes { get; set; }
    public int? OvertimeHours { get; set; }
    public string? Note { get; set; }
    public string? CheckInLocation { get; set; }
    public string? CheckOutLocation { get; set; }
}
