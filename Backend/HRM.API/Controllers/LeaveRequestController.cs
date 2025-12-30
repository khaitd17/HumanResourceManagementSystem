using HRM.ServiceLayer.DTOs.LeaveRequest;
using HRM.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _leaveRequestService.GetByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var result = await _leaveRequestService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _leaveRequestService.GetPendingRequestsAsync();
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var result = await _leaveRequestService.GetByStatusAsync(status);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        var result = await _leaveRequestService.CreateAsync(dto);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPost("approve")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Approve([FromBody] ApproveLeaveRequestDto dto)
    {
        var result = await _leaveRequestService.ApproveAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _leaveRequestService.DeleteAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        var employeeId = int.Parse(User.FindFirst("EmployeeId")?.Value ?? "0");
        if (employeeId == 0) return BadRequest(new { message = "Employee ID not found" });
        
        var result = await _leaveRequestService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }
}
