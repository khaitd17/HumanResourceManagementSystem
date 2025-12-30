using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.Employee;

namespace HRM.ServiceLayer.Interfaces;

public interface IEmployeeService
{
    Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id);
    Task<ServiceResult<EmployeeDto>> GetByEmployeeCodeAsync(string employeeCode);
    Task<ServiceResult<List<EmployeeDto>>> GetAllAsync();
    Task<ServiceResult<List<EmployeeDto>>> GetByDepartmentAsync(int departmentId);
    Task<ServiceResult<List<EmployeeDto>>> GetActiveEmployeesAsync();
    Task<ServiceResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto);
    Task<ServiceResult<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult> ActivateAsync(int id);
    Task<ServiceResult> DeactivateAsync(int id);
}
