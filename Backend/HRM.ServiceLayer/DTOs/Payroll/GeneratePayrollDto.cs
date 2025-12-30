namespace HRM.ServiceLayer.DTOs.Payroll;

public class GeneratePayrollDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<int>? EmployeeIds { get; set; } // null = all employees
}

public class GeneratePayrollResultDto
{
    public int TotalEmployees { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
