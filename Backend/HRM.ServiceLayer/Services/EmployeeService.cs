using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.Employee;
using HRM.ServiceLayer.Interfaces;

namespace HRM.ServiceLayer.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResult<EmployeeDto>.FailureResult("Employee not found");
            }

            var dto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<EmployeeDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<EmployeeDto>> GetByEmployeeCodeAsync(string employeeCode)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByEmployeeCodeAsync(employeeCode);
            if (employee == null)
            {
                return ServiceResult<EmployeeDto>.FailureResult("Employee not found");
            }

            var dto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<EmployeeDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<EmployeeDto>>> GetAllAsync()
    {
        try
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            var dtos = _mapper.Map<List<EmployeeDto>>(employees);
            return ServiceResult<List<EmployeeDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<EmployeeDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<EmployeeDto>>> GetByDepartmentAsync(int departmentId)
    {
        try
        {
            var employees = await _unitOfWork.Employees.GetByDepartmentAsync(departmentId);
            var dtos = _mapper.Map<List<EmployeeDto>>(employees);
            return ServiceResult<List<EmployeeDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<EmployeeDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<EmployeeDto>>> GetActiveEmployeesAsync()
    {
        try
        {
            var employees = await _unitOfWork.Employees.GetActiveEmployeesAsync();
            var dtos = _mapper.Map<List<EmployeeDto>>(employees);
            return ServiceResult<List<EmployeeDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<EmployeeDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto)
    {
        try
        {
            // Check if employee code exists
            if (await _unitOfWork.Employees.EmployeeCodeExistsAsync(dto.EmployeeCode))
            {
                return ServiceResult<EmployeeDto>.FailureResult("Employee code already exists");
            }

            // Check if email exists
            if (await _unitOfWork.Employees.EmailExistsAsync(dto.Email))
            {
                return ServiceResult<EmployeeDto>.FailureResult("Email already exists");
            }

            var employee = _mapper.Map<Employee>(dto);
            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.SuccessResult(result, "Employee created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<EmployeeDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);
            if (employee == null)
            {
                return ServiceResult<EmployeeDto>.FailureResult("Employee not found");
            }

            // Check if email exists for another employee
            var existingEmployee = await _unitOfWork.Employees.GetByEmailAsync(dto.Email);
            if (existingEmployee != null && existingEmployee.Id != dto.Id)
            {
                return ServiceResult<EmployeeDto>.FailureResult("Email already exists");
            }

            _mapper.Map(dto, employee);
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.SuccessResult(result, "Employee updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<EmployeeDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResult.FailureResult("Employee not found");
            }

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Employee deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ActivateAsync(int id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResult.FailureResult("Employee not found");
            }

            employee.IsActive = true;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Employee activated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeactivateAsync(int id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResult.FailureResult("Employee not found");
            }

            employee.IsActive = false;
            employee.ResignDate = DateTime.UtcNow;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Employee deactivated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }
}
