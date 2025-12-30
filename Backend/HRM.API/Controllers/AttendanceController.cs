using HRM.ServiceLayer.DTOs.Attendance;
using HRM.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// Check-in for current user
    /// </summary>
    [HttpPost("check-in")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
    {
        var result = await _attendanceService.CheckInAsync(dto);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(result);
    }

    /// <summary>
    /// Check-out for current user
    /// </summary>
    [HttpPost("check-out")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
    {
        var result = await _attendanceService.CheckOutAsync(dto);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get attendance by employee and date
    /// </summary>
    [HttpGet("employee/{employeeId}/date/{date}")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployeeAndDate(int employeeId, DateTime date)
    {
        var result = await _attendanceService.GetByEmployeeAndDateAsync(employeeId, date);

        if (!result.Success)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get attendance by employee and month
    /// </summary>
    [HttpGet("employee/{employeeId}/month/{year}/{month}")]
    [ProducesResponseType(typeof(List<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployeeAndMonth(int employeeId, int year, int month)
    {
        var result = await _attendanceService.GetByEmployeeAndMonthAsync(employeeId, year, month);
        return Ok(result);
    }

    /// <summary>
    /// Get all attendance for a specific date
    /// </summary>
    [HttpGet("date/{date}")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(typeof(List<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        var result = await _attendanceService.GetByDateAsync(date);
        return Ok(result);
    }

    /// <summary>
    /// Import attendance from Excel file
    /// </summary>
    [HttpPost("import")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(typeof(ImportAttendanceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
        {
            return BadRequest(new { message = "Invalid file format. Only Excel files are allowed" });
        }

        using var stream = file.OpenReadStream();
        var result = await _attendanceService.ImportFromExcelAsync(stream);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Ok(result);
    }

    /// <summary>
    /// Export attendance to Excel file
    /// </summary>
    [HttpGet("export/{year}/{month}")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportToExcel(int year, int month)
    {
        var result = await _attendanceService.ExportToExcelAsync(year, month);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return File(result.Data!, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Attendance_{year}_{month}.xlsx");
    }

    /// <summary>
    /// Get my attendance for today
    /// </summary>
    [HttpGet("my-today")]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyToday()
    {
        var employeeId = int.Parse(User.FindFirst("EmployeeId")?.Value ?? "0");

        if (employeeId == 0)
        {
            return BadRequest(new { message = "Employee ID not found" });
        }

        var result = await _attendanceService.GetByEmployeeAndDateAsync(employeeId, DateTime.Today);

        if (!result.Success)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(result);
    }
}
