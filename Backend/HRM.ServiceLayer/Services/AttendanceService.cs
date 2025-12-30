using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Attendance;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.Interfaces;
using OfficeOpenXml;

namespace HRM.ServiceLayer.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<ServiceResult<AttendanceDto>> CheckInAsync(CheckInDto dto)
    {
        try
        {
            // Check if already checked in today
            var existing = await _unitOfWork.Attendances.GetByEmployeeAndDateAsync(dto.EmployeeId, dto.WorkDate);
            if (existing != null)
            {
                return ServiceResult<AttendanceDto>.FailureResult("Already checked in today");
            }

            var attendance = _mapper.Map<Attendance>(dto);
            
            // Calculate late minutes (assuming work starts at 8:00 AM)
            var standardStartTime = new TimeSpan(8, 0, 0);
            if (dto.CheckInTime > standardStartTime)
            {
                attendance.LateMinutes = (int)(dto.CheckInTime - standardStartTime).TotalMinutes;
                attendance.Status = "Late";
            }

            await _unitOfWork.Attendances.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<AttendanceDto>(attendance);
            return ServiceResult<AttendanceDto>.SuccessResult(result, "Check-in successful");
        }
        catch (Exception ex)
        {
            return ServiceResult<AttendanceDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<AttendanceDto>> CheckOutAsync(CheckOutDto dto)
    {
        try
        {
            var attendance = await _unitOfWork.Attendances.GetByEmployeeAndDateAsync(dto.EmployeeId, dto.WorkDate);
            if (attendance == null)
            {
                return ServiceResult<AttendanceDto>.FailureResult("No check-in record found");
            }

            if (attendance.CheckOut.HasValue)
            {
                return ServiceResult<AttendanceDto>.FailureResult("Already checked out");
            }

            attendance.CheckOut = dto.CheckOutTime;
            attendance.CheckOutLocation = dto.Location;
            
            // Calculate total hours
            if (attendance.CheckIn.HasValue)
            {
                var totalMinutes = (dto.CheckOutTime - attendance.CheckIn.Value).TotalMinutes;
                attendance.TotalHours = (decimal)(totalMinutes / 60);

                // Calculate early leave (assuming work ends at 17:00)
                var standardEndTime = new TimeSpan(17, 0, 0);
                if (dto.CheckOutTime < standardEndTime)
                {
                    attendance.EarlyLeaveMinutes = (int)(standardEndTime - dto.CheckOutTime).TotalMinutes;
                }

                // Calculate overtime
                if (dto.CheckOutTime > standardEndTime)
                {
                    attendance.OvertimeHours = (int)(dto.CheckOutTime - standardEndTime).TotalHours;
                }
            }

            _unitOfWork.Attendances.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<AttendanceDto>(attendance);
            return ServiceResult<AttendanceDto>.SuccessResult(result, "Check-out successful");
        }
        catch (Exception ex)
        {
            return ServiceResult<AttendanceDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<AttendanceDto>> GetByEmployeeAndDateAsync(int employeeId, DateTime date)
    {
        try
        {
            var attendance = await _unitOfWork.Attendances.GetByEmployeeAndDateAsync(employeeId, date);
            if (attendance == null)
            {
                return ServiceResult<AttendanceDto>.FailureResult("Attendance record not found");
            }

            var dto = _mapper.Map<AttendanceDto>(attendance);
            return ServiceResult<AttendanceDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AttendanceDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<AttendanceDto>>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month)
    {
        try
        {
            var attendances = await _unitOfWork.Attendances.GetByEmployeeAndMonthAsync(employeeId, year, month);
            var dtos = _mapper.Map<List<AttendanceDto>>(attendances);
            return ServiceResult<List<AttendanceDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AttendanceDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<AttendanceDto>>> GetByDateAsync(DateTime date)
    {
        try
        {
            var attendances = await _unitOfWork.Attendances.GetByDateAsync(date);
            var dtos = _mapper.Map<List<AttendanceDto>>(attendances);
            return ServiceResult<List<AttendanceDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AttendanceDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ImportAttendanceResultDto>> ImportFromExcelAsync(Stream fileStream)
    {
        var result = new ImportAttendanceResultDto();
        var errors = new List<ImportErrorDto>();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Skip header
            {
                try
                {
                    var employeeCode = worksheet.Cells[row, 1].Value?.ToString();
                    var workDate = DateTime.Parse(worksheet.Cells[row, 2].Value?.ToString() ?? "");
                    var checkIn = TimeSpan.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "");
                    var checkOut = worksheet.Cells[row, 4].Value?.ToString();

                    if (string.IsNullOrEmpty(employeeCode))
                    {
                        errors.Add(new ImportErrorDto { RowNumber = row, EmployeeCode = "", ErrorMessage = "Employee code is required" });
                        continue;
                    }

                    var employee = await _unitOfWork.Employees.GetByEmployeeCodeAsync(employeeCode);
                    if (employee == null)
                    {
                        errors.Add(new ImportErrorDto { RowNumber = row, EmployeeCode = employeeCode, ErrorMessage = "Employee not found" });
                        continue;
                    }

                    var existing = await _unitOfWork.Attendances.GetByEmployeeAndDateAsync(employee.Id, workDate);
                    if (existing != null)
                    {
                        // Update existing
                        existing.CheckIn = checkIn;
                        existing.CheckOut = string.IsNullOrEmpty(checkOut) ? null : TimeSpan.Parse(checkOut);
                        if (existing.CheckIn.HasValue && existing.CheckOut.HasValue)
                        {
                            existing.TotalHours = (decimal)(existing.CheckOut.Value - existing.CheckIn.Value).TotalHours;
                        }
                        _unitOfWork.Attendances.Update(existing);
                    }
                    else
                    {
                        // Create new
                        var attendance = new Attendance
                        {
                            EmployeeId = employee.Id,
                            WorkDate = workDate,
                            CheckIn = checkIn,
                            CheckOut = string.IsNullOrEmpty(checkOut) ? null : TimeSpan.Parse(checkOut),
                            Status = "Present"
                        };

                        if (attendance.CheckIn.HasValue && attendance.CheckOut.HasValue)
                        {
                            attendance.TotalHours = (decimal)(attendance.CheckOut.Value - attendance.CheckIn.Value).TotalHours;
                        }

                        await _unitOfWork.Attendances.AddAsync(attendance);
                    }

                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportErrorDto 
                    { 
                        RowNumber = row, 
                        EmployeeCode = worksheet.Cells[row, 1].Value?.ToString() ?? "", 
                        ErrorMessage = ex.Message 
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            result.TotalRows = rowCount - 1;
            result.ErrorCount = errors.Count;
            result.Errors = errors;

            return ServiceResult<ImportAttendanceResultDto>.SuccessResult(result, "Import completed");
        }
        catch (Exception ex)
        {
            return ServiceResult<ImportAttendanceResultDto>.FailureResult($"Import failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult<byte[]>> ExportToExcelAsync(int year, int month)
    {
        try
        {
            var attendances = await _unitOfWork.Attendances.GetByEmployeeAndMonthAsync(0, year, month);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add($"Attendance_{year}_{month}");

            // Headers
            worksheet.Cells[1, 1].Value = "Employee Code";
            worksheet.Cells[1, 2].Value = "Employee Name";
            worksheet.Cells[1, 3].Value = "Date";
            worksheet.Cells[1, 4].Value = "Check In";
            worksheet.Cells[1, 5].Value = "Check Out";
            worksheet.Cells[1, 6].Value = "Total Hours";
            worksheet.Cells[1, 7].Value = "Status";

            // Data
            int row = 2;
            foreach (var attendance in attendances)
            {
                worksheet.Cells[row, 1].Value = attendance.Employee.EmployeeCode;
                worksheet.Cells[row, 2].Value = attendance.Employee.FullName;
                worksheet.Cells[row, 3].Value = attendance.WorkDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = attendance.CheckIn?.ToString(@"hh\:mm");
                worksheet.Cells[row, 5].Value = attendance.CheckOut?.ToString(@"hh\:mm");
                worksheet.Cells[row, 6].Value = attendance.TotalHours;
                worksheet.Cells[row, 7].Value = attendance.Status;
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            var bytes = package.GetAsByteArray();
            return ServiceResult<byte[]>.SuccessResult(bytes, "Export successful");
        }
        catch (Exception ex)
        {
            return ServiceResult<byte[]>.FailureResult($"Export failed: {ex.Message}");
        }
    }
}
