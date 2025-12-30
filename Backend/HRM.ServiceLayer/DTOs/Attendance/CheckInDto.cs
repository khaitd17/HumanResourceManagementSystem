namespace HRM.ServiceLayer.DTOs.Attendance;

public class CheckInDto
{
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public TimeSpan CheckInTime { get; set; }
    public string? Location { get; set; } // GPS/IP
    public string? Note { get; set; }
}
