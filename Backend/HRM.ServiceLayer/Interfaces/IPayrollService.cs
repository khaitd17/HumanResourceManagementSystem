using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.Payroll;

namespace HRM.ServiceLayer.Interfaces;

public interface IPayrollService
{
    Task<ServiceResult<PayrollDto>> GetByIdAsync(int id);
    Task<ServiceResult<PayrollDto>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month);
    Task<ServiceResult<List<PayrollDto>>> GetByMonthAsync(int year, int month);
    Task<ServiceResult<List<PayrollDto>>> GetByEmployeeAsync(int employeeId);
    Task<ServiceResult<GeneratePayrollResultDto>> GeneratePayrollAsync(GeneratePayrollDto dto);
    Task<ServiceResult<PayrollDto>> RecalculateAsync(int payrollId);
    Task<ServiceResult> ApproveAsync(int payrollId);
    Task<ServiceResult<byte[]>> ExportPayslipPdfAsync(int payrollId);
    Task<ServiceResult> DeleteAsync(int id);
}
