using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Chat;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.Interfaces;

namespace HRM.ServiceLayer.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<ChatRoomDto>> GetChatRoomByIdAsync(int chatRoomId)
    {
        try
        {
            var chatRoom = await _unitOfWork.ChatRooms.GetWithMembersAsync(chatRoomId);
            if (chatRoom == null)
            {
                return ServiceResult<ChatRoomDto>.FailureResult("Chat room not found");
            }

            var dto = _mapper.Map<ChatRoomDto>(chatRoom);
            
            // Get last message
            var messages = await _unitOfWork.Messages.GetByChatRoomAsync(chatRoomId, 1, 1);
            if (messages.Any())
            {
                dto.LastMessage = _mapper.Map<MessageDto>(messages.First());
            }

            return ServiceResult<ChatRoomDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<ChatRoomDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<ChatRoomDto>>> GetChatRoomsByEmployeeAsync(int employeeId)
    {
        try
        {
            var chatRooms = await _unitOfWork.ChatRooms.GetByEmployeeAsync(employeeId);
            var dtos = new List<ChatRoomDto>();

            foreach (var chatRoom in chatRooms)
            {
                var dto = _mapper.Map<ChatRoomDto>(chatRoom);
                
                // Get last message
                var messages = await _unitOfWork.Messages.GetByChatRoomAsync(chatRoom.Id, 1, 1);
                if (messages.Any())
                {
                    dto.LastMessage = _mapper.Map<MessageDto>(messages.First());
                }

                // Get unread count
                dto.UnreadCount = await _unitOfWork.Messages.GetUnreadCountAsync(chatRoom.Id, employeeId);

                dtos.Add(dto);
            }

            // Sort by last message time
            dtos = dtos.OrderByDescending(x => x.LastMessage?.CreatedAt ?? x.CreatedAt).ToList();

            return ServiceResult<List<ChatRoomDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ChatRoomDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto dto)
    {
        try
        {
            // Validate members exist
            foreach (var memberId in dto.MemberIds)
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(memberId);
                if (employee == null)
                {
                    return ServiceResult<ChatRoomDto>.FailureResult($"Employee with ID {memberId} not found");
                }
            }

            // Create chat room
            var chatRoom = new ChatRoom
            {
                Name = dto.Name,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ChatRooms.AddAsync(chatRoom);
            await _unitOfWork.SaveChangesAsync();

            // Add members
            foreach (var memberId in dto.MemberIds)
            {
                var member = new ChatRoomMember
                {
                    ChatRoomId = chatRoom.Id,
                    EmployeeId = memberId,
                    JoinedAt = DateTime.UtcNow
                };
                await _unitOfWork.ChatRoomMembers.AddAsync(member);
            }

            await _unitOfWork.SaveChangesAsync();

            // Reload with members
            var createdRoom = await _unitOfWork.ChatRooms.GetWithMembersAsync(chatRoom.Id);
            var result = _mapper.Map<ChatRoomDto>(createdRoom);

            return ServiceResult<ChatRoomDto>.SuccessResult(result, "Chat room created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ChatRoomDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ChatRoomDto>> GetOrCreateDirectChatAsync(int employee1Id, int employee2Id)
    {
        try
        {
            // Validate employees exist
            var employee1 = await _unitOfWork.Employees.GetByIdAsync(employee1Id);
            var employee2 = await _unitOfWork.Employees.GetByIdAsync(employee2Id);

            if (employee1 == null || employee2 == null)
            {
                return ServiceResult<ChatRoomDto>.FailureResult("One or both employees not found");
            }

            // Check if direct chat already exists
            var existingChat = await _unitOfWork.ChatRooms.GetDirectChatRoomAsync(employee1Id, employee2Id);
            
            if (existingChat != null)
            {
                var existingDto = _mapper.Map<ChatRoomDto>(existingChat);
                
                // Get last message
                var messages = await _unitOfWork.Messages.GetByChatRoomAsync(existingChat.Id, 1, 1);
                if (messages.Any())
                {
                    existingDto.LastMessage = _mapper.Map<MessageDto>(messages.First());
                }

                return ServiceResult<ChatRoomDto>.SuccessResult(existingDto);
            }

            // Create new direct chat
            var chatRoom = new ChatRoom
            {
                Name = $"{employee1.FullName} - {employee2.FullName}",
                Type = "Direct",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ChatRooms.AddAsync(chatRoom);
            await _unitOfWork.SaveChangesAsync();

            // Add members
            await _unitOfWork.ChatRoomMembers.AddAsync(new ChatRoomMember
            {
                ChatRoomId = chatRoom.Id,
                EmployeeId = employee1Id,
                JoinedAt = DateTime.UtcNow
            });

            await _unitOfWork.ChatRoomMembers.AddAsync(new ChatRoomMember
            {
                ChatRoomId = chatRoom.Id,
                EmployeeId = employee2Id,
                JoinedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();

            // Reload with members
            var createdRoom = await _unitOfWork.ChatRooms.GetWithMembersAsync(chatRoom.Id);
            var result = _mapper.Map<ChatRoomDto>(createdRoom);

            return ServiceResult<ChatRoomDto>.SuccessResult(result, "Direct chat created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ChatRoomDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<List<MessageDto>>> GetMessagesAsync(int chatRoomId, int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            // Validate chat room exists
            var chatRoom = await _unitOfWork.ChatRooms.GetByIdAsync(chatRoomId);
            if (chatRoom == null)
            {
                return ServiceResult<List<MessageDto>>.FailureResult("Chat room not found");
            }

            var messages = await _unitOfWork.Messages.GetByChatRoomAsync(chatRoomId, pageNumber, pageSize);
            var dtos = _mapper.Map<List<MessageDto>>(messages);

            // Reverse to show oldest first
            dtos.Reverse();

            return ServiceResult<List<MessageDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<MessageDto>>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<MessageDto>> SendMessageAsync(SendMessageDto dto)
    {
        try
        {
            // Validate chat room exists
            var chatRoom = await _unitOfWork.ChatRooms.GetByIdAsync(dto.ChatRoomId);
            if (chatRoom == null)
            {
                return ServiceResult<MessageDto>.FailureResult("Chat room not found");
            }

            // Validate sender is a member
            var isMember = await _unitOfWork.ChatRoomMembers.IsMemberAsync(dto.ChatRoomId, dto.SenderId);
            if (!isMember)
            {
                return ServiceResult<MessageDto>.FailureResult("You are not a member of this chat room");
            }

            // Validate sender exists
            var sender = await _unitOfWork.Employees.GetByIdAsync(dto.SenderId);
            if (sender == null)
            {
                return ServiceResult<MessageDto>.FailureResult("Sender not found");
            }

            var message = _mapper.Map<Message>(dto);
            
            await _unitOfWork.Messages.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            // Reload with sender info
            var savedMessage = await _unitOfWork.Messages.GetByIdAsync(message.Id);
            var result = _mapper.Map<MessageDto>(savedMessage);

            // TODO: Send real-time notification via SignalR

            return ServiceResult<MessageDto>.SuccessResult(result, "Message sent successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<MessageDto>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult> MarkAsReadAsync(int messageId)
    {
        try
        {
            await _unitOfWork.Messages.MarkAsReadAsync(messageId);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.SuccessResult("Message marked as read");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<ServiceResult<int>> GetUnreadCountAsync(int chatRoomId, int employeeId)
    {
        try
        {
            var count = await _unitOfWork.Messages.GetUnreadCountAsync(chatRoomId, employeeId);
            return ServiceResult<int>.SuccessResult(count);
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.FailureResult($"Error: {ex.Message}");
        }
    }
}
