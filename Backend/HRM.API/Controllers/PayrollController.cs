using HRM.ServiceLayer.DTOs.Payroll;
using HRM.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _payrollService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}/month/{year}/{month}")]
    public async Task<IActionResult> GetByEmployeeAndMonth(int employeeId, int year, int month)
    {
        var result = await _payrollService.GetByEmployeeAndMonthAsync(employeeId, year, month);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("month/{year}/{month}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetByMonth(int year, int month)
    {
        var result = await _payrollService.GetByMonthAsync(year, month);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var result = await _payrollService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Generate([FromBody] GeneratePayrollDto dto)
    {
        var result = await _payrollService.GeneratePayrollAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/recalculate")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Recalculate(int id)
    {
        var result = await _payrollService.RecalculateAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _payrollService.ApproveAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}/export-pdf")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var result = await _payrollService.ExportPayslipPdfAsync(id);
        if (!result.Success) return BadRequest(result);
        return File(result.Data!, "application/pdf", $"Payslip_{id}.pdf");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _payrollService.DeleteAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
