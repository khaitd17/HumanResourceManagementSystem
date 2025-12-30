using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.DTOs.Payroll;
using HRM.ServiceLayer.Interfaces;

namespace HRM.ServiceLayer.Services;

public class PayrollService : IPayrollService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PayrollService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PayrollDto>> GetByIdAsync(int id)
    {
        try
        {
            var payroll = await _unitOfWork.Payrolls.GetByIdAsync(id);
            if (payroll == null)
            {
                return ServiceResult<PayrollDto>.FailureResult("Payroll not found");
            }

            var dto = _mapper.Map<PayrollDto>(payroll);
            return ServiceResult<PayrollDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<PayrollDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<PayrollDto>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month)
    {
        try
        {
            var payroll = await _unitOfWork.Payrolls.GetByEmployeeAndMonthAsync(employeeId, year, month);
            if (payroll == null)
            {
                return ServiceResult<PayrollDto>.FailureResult("Payroll not found");
            }

            var dto = _mapper.Map<PayrollDto>(payroll);
            return ServiceResult<PayrollDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<PayrollDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<PayrollDto>>> GetByMonthAsync(int year, int month)
    {
        try
        {
            var payrolls = await _unitOfWork.Payrolls.GetByMonthAsync(year, month);
            var dtos = _mapper.Map<List<PayrollDto>>(payrolls);
            return ServiceResult<List<PayrollDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PayrollDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<PayrollDto>>> GetByEmployeeAsync(int employeeId)
    {
        try
        {
            var payrolls = await _unitOfWork.Payrolls.GetByEmployeeAsync(employeeId);
            var dtos = _mapper.Map<List<PayrollDto>>(payrolls);
            return ServiceResult<List<PayrollDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PayrollDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<GeneratePayrollResultDto>> GeneratePayrollAsync(GeneratePayrollDto dto)
    {
        var result = new GeneratePayrollResultDto();
        var errors = new List<string>();

        try
        {
            // Get payroll config
            var config = await _unitOfWork.PayrollConfigs.GetActiveConfigAsync();
            if (config == null)
            {
                return ServiceResult<GeneratePayrollResultDto>.FailureResult("No active payroll configuration found");
            }

            // Get employees
            IEnumerable<Employee> employees;
            if (dto.EmployeeIds != null && dto.EmployeeIds.Any())
            {
                employees = new List<Employee>();
                foreach (var empId in dto.EmployeeIds)
                {
                    var emp = await _unitOfWork.Employees.GetByIdAsync(empId);
                    if (emp != null && emp.IsActive)
                    {
                        ((List<Employee>)employees).Add(emp);
                    }
                }
            }
            else
            {
                employees = await _unitOfWork.Employees.GetActiveEmployeesAsync();
            }

            result.TotalEmployees = employees.Count();

            foreach (var employee in employees)
            {
                try
                {
                    // Check if payroll already exists
                    if (await _unitOfWork.Payrolls.PayrollExistsAsync(employee.Id, dto.Year, dto.Month))
                    {
                        errors.Add($"Payroll already exists for {employee.FullName} ({employee.EmployeeCode})");
                        result.ErrorCount++;
                        continue;
                    }

                    // Calculate payroll
                    var payroll = await CalculatePayrollAsync(employee, dto.Year, dto.Month, config);
                    
                    await _unitOfWork.Payrolls.AddAsync(payroll);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Error for {employee.FullName}: {ex.Message}");
                    result.ErrorCount++;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            result.Errors = errors;
            return ServiceResult<GeneratePayrollResultDto>.SuccessResult(result, "Payroll generation completed");
        }
        catch (Exception ex)
        {
            return ServiceResult<GeneratePayrollResultDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<PayrollDto>> RecalculateAsync(int payrollId)
    {
        try
        {
            var payroll = await _unitOfWork.Payrolls.GetByIdAsync(payrollId);
            if (payroll == null)
            {
                return ServiceResult<PayrollDto>.FailureResult("Payroll not found");
            }

            if (payroll.Status == "Paid")
            {
                return ServiceResult<PayrollDto>.FailureResult("Cannot recalculate paid payroll");
            }

            var config = await _unitOfWork.PayrollConfigs.GetActiveConfigAsync();
            if (config == null)
            {
                return ServiceResult<PayrollDto>.FailureResult("No active payroll configuration found");
            }

            var employee = await _unitOfWork.Employees.GetByIdAsync(payroll.EmployeeId);
            if (employee == null)
            {
                return ServiceResult<PayrollDto>.FailureResult("Employee not found");
            }

            // Recalculate
            var recalculated = await CalculatePayrollAsync(employee, payroll.Year, payroll.Month, config);
            
            // Update existing payroll
            payroll.BaseSalary = recalculated.BaseSalary;
            payroll.ActualWorkingDays = recalculated.ActualWorkingDays;
            payroll.InsuranceSalary = recalculated.InsuranceSalary;
            payroll.CompanyInsurance = recalculated.CompanyInsurance;
            payroll.EmployeeInsurance = recalculated.EmployeeInsurance;
            payroll.PersonalIncomeTax = recalculated.PersonalIncomeTax;
            payroll.NetSalary = recalculated.NetSalary;
            payroll.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Payrolls.Update(payroll);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<PayrollDto>(payroll);
            return ServiceResult<PayrollDto>.SuccessResult(dto, "Payroll recalculated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<PayrollDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ApproveAsync(int payrollId)
    {
        try
        {
            var payroll = await _unitOfWork.Payrolls.GetByIdAsync(payrollId);
            if (payroll == null)
            {
                return ServiceResult.FailureResult("Payroll not found");
            }

            if (payroll.Status == "Paid")
            {
                return ServiceResult.FailureResult("Payroll is already paid");
            }

            payroll.Status = "Approved";
            payroll.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Payrolls.Update(payroll);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Payroll approved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<byte[]>> ExportPayslipPdfAsync(int payrollId)
    {
        try
        {
            // TODO: Implement PDF generation using a library like QuestPDF or iTextSharp
            // For now, return a placeholder
            await Task.CompletedTask;
            return ServiceResult<byte[]>.FailureResult("PDF export not implemented yet");
        }
        catch (Exception ex)
        {
            return ServiceResult<byte[]>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var payroll = await _unitOfWork.Payrolls.GetByIdAsync(id);
            if (payroll == null)
            {
                return ServiceResult.FailureResult("Payroll not found");
            }

            if (payroll.Status == "Paid")
            {
                return ServiceResult.FailureResult("Cannot delete paid payroll");
            }

            // Soft delete
            payroll.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Payrolls.Update(payroll);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Payroll deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private async Task<Payroll> CalculatePayrollAsync(Employee employee, int year, int month, PayrollConfig config)
    {
        // Get actual working days from attendance
        var actualWorkingDays = await _unitOfWork.Attendances.GetWorkingDaysInMonthAsync(employee.Id, year, month);

        // Calculate base salary for actual days worked
        var dailySalary = employee.BaseSalary / config.StandardWorkingDays;
        var calculatedBaseSalary = dailySalary * actualWorkingDays;

        // Insurance salary (usually base salary)
        var insuranceSalary = employee.BaseSalary;

        // Calculate insurance
        var companyInsurance = insuranceSalary * (config.CompanyInsuranceRate / 100);
        var employeeInsurance = insuranceSalary * (config.EmployeeInsuranceRate / 100);

        // Calculate gross salary
        var grossSalary = calculatedBaseSalary;

        // Calculate taxable income
        var taxableIncome = grossSalary - employeeInsurance - config.PersonalTaxDeduction;

        // Calculate personal income tax (simplified progressive tax)
        var personalIncomeTax = CalculatePersonalIncomeTax(taxableIncome);

        // Calculate net salary
        var netSalary = grossSalary - employeeInsurance - personalIncomeTax;

        return new Payroll
        {
            EmployeeId = employee.Id,
            Month = month,
            Year = year,
            BaseSalary = employee.BaseSalary,
            KpiBonus = 0, // Can be set manually later
            ResponsibilityAllowance = 0,
            LunchAllowance = 0,
            PhoneAllowance = 0,
            TravelAllowance = 0,
            StandardWorkingDays = config.StandardWorkingDays,
            ActualWorkingDays = actualWorkingDays,
            InsuranceSalary = insuranceSalary,
            CompanyInsurance = companyInsurance,
            EmployeeInsurance = employeeInsurance,
            PersonalIncomeTax = personalIncomeTax,
            NetSalary = netSalary,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };
    }

    private decimal CalculatePersonalIncomeTax(decimal taxableIncome)
    {
        if (taxableIncome <= 0) return 0;

        // Vietnam progressive tax rates (simplified)
        decimal tax = 0;

        if (taxableIncome <= 5000000) // 0-5M: 5%
        {
            tax = taxableIncome * 0.05m;
        }
        else if (taxableIncome <= 10000000) // 5M-10M: 10%
        {
            tax = 5000000 * 0.05m + (taxableIncome - 5000000) * 0.10m;
        }
        else if (taxableIncome <= 18000000) // 10M-18M: 15%
        {
            tax = 5000000 * 0.05m + 5000000 * 0.10m + (taxableIncome - 10000000) * 0.15m;
        }
        else if (taxableIncome <= 32000000) // 18M-32M: 20%
        {
            tax = 5000000 * 0.05m + 5000000 * 0.10m + 8000000 * 0.15m + (taxableIncome - 18000000) * 0.20m;
        }
        else if (taxableIncome <= 52000000) // 32M-52M: 25%
        {
            tax = 5000000 * 0.05m + 5000000 * 0.10m + 8000000 * 0.15m + 14000000 * 0.20m + (taxableIncome - 32000000) * 0.25m;
        }
        else if (taxableIncome <= 80000000) // 52M-80M: 30%
        {
            tax = 5000000 * 0.05m + 5000000 * 0.10m + 8000000 * 0.15m + 14000000 * 0.20m + 20000000 * 0.25m + (taxableIncome - 52000000) * 0.30m;
        }
        else // > 80M: 35%
        {
            tax = 5000000 * 0.05m + 5000000 * 0.10m + 8000000 * 0.15m + 14000000 * 0.20m + 20000000 * 0.25m + 28000000 * 0.30m + (taxableIncome - 80000000) * 0.35m;
        }

        return Math.Round(tax, 0);
    }

    #endregion
}
