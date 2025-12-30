namespace HRM.ServiceLayer.DTOs.Payroll;

public class PayrollDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? DepartmentName { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal KpiBonus { get; set; }
    public decimal ResponsibilityAllowance { get; set; }
    public decimal LunchAllowance { get; set; }
    public decimal PhoneAllowance { get; set; }
    public decimal TravelAllowance { get; set; }
    public int StandardWorkingDays { get; set; }
    public int ActualWorkingDays { get; set; }
    public decimal InsuranceSalary { get; set; }
    public decimal CompanyInsurance { get; set; }
    public decimal EmployeeInsurance { get; set; }
    public decimal PersonalIncomeTax { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
