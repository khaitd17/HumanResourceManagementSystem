using HRM.ServiceLayer.DTOs.Chat;
using HRM.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("rooms/{id}")]
    public async Task<IActionResult> GetChatRoomById(int id)
    {
        var result = await _chatService.GetChatRoomByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("rooms/employee/{employeeId}")]
    public async Task<IActionResult> GetChatRoomsByEmployee(int employeeId)
    {
        var result = await _chatService.GetChatRoomsByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomDto dto)
    {
        var result = await _chatService.CreateChatRoomAsync(dto);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetChatRoomById), new { id = result.Data?.Id }, result);
    }

    [HttpPost("rooms/direct/{employee1Id}/{employee2Id}")]
    public async Task<IActionResult> GetOrCreateDirectChat(int employee1Id, int employee2Id)
    {
        var result = await _chatService.GetOrCreateDirectChatAsync(employee1Id, employee2Id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("rooms/{chatRoomId}/messages")]
    public async Task<IActionResult> GetMessages(int chatRoomId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _chatService.GetMessagesAsync(chatRoomId, pageNumber, pageSize);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var result = await _chatService.SendMessageAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("messages/{messageId}/read")]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        var result = await _chatService.MarkAsReadAsync(messageId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("rooms/{chatRoomId}/unread/{employeeId}")]
    public async Task<IActionResult> GetUnreadCount(int chatRoomId, int employeeId)
    {
        var result = await _chatService.GetUnreadCountAsync(chatRoomId, employeeId);
        return Ok(result);
    }
}
