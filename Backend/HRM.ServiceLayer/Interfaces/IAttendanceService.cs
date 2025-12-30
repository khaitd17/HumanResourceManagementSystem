using HRM.ServiceLayer.DTOs.Attendance;
using HRM.ServiceLayer.DTOs.Common;

namespace HRM.ServiceLayer.Interfaces;

public interface IAttendanceService
{
    Task<ServiceResult<AttendanceDto>> CheckInAsync(CheckInDto dto);
    Task<ServiceResult<AttendanceDto>> CheckOutAsync(CheckOutDto dto);
    Task<ServiceResult<AttendanceDto>> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
    Task<ServiceResult<List<AttendanceDto>>> GetByEmployeeAndMonthAsync(int employeeId, int year, int month);
    Task<ServiceResult<List<AttendanceDto>>> GetByDateAsync(DateTime date);
    Task<ServiceResult<ImportAttendanceResultDto>> ImportFromExcelAsync(Stream fileStream);
    Task<ServiceResult<byte[]>> ExportToExcelAsync(int year, int month);
}
