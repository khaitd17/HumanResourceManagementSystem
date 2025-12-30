namespace HRM.ServiceLayer.DTOs.Attendance;

public class ImportAttendanceDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string? Note { get; set; }
}

public class ImportAttendanceResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();
}

public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
