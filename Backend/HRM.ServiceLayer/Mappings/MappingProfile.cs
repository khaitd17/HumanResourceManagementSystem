using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.ServiceLayer.DTOs.Auth;
using HRM.ServiceLayer.DTOs.Employee;
using HRM.ServiceLayer.DTOs.Department;
using HRM.ServiceLayer.DTOs.Attendance;
using HRM.ServiceLayer.DTOs.LeaveRequest;
using HRM.ServiceLayer.DTOs.Payroll;
using HRM.ServiceLayer.DTOs.Chat;

namespace HRM.ServiceLayer.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Email : null));

        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        // Employee Mappings
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.EmployeeCode, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Department Mappings
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.EmployeeCount, opt => opt.MapFrom(src => src.Employees.Count));

        // Attendance Mappings
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

        CreateMap<CheckInDto, Attendance>()
            .ForMember(dest => dest.CheckIn, opt => opt.MapFrom(src => src.CheckInTime))
            .ForMember(dest => dest.CheckInLocation, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Present"));

        // LeaveRequest Mappings
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.ApprovedByName, opt => opt.Ignore());

        CreateMap<CreateLeaveRequestDto, LeaveRequest>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.TotalDays, opt => opt.MapFrom(src => (decimal)(src.ToDate - src.FromDate).TotalDays + 1));

        // Payroll Mappings
        CreateMap<Payroll, PayrollDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee.EmployeeCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Employee.Department != null ? src.Employee.Department.Name : null));

        // Chat Mappings
        CreateMap<ChatRoom, ChatRoomDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.ChatRoomMembers))
            .ForMember(dest => dest.LastMessage, opt => opt.Ignore())
            .ForMember(dest => dest.UnreadCount, opt => opt.Ignore());

        CreateMap<ChatRoomMember, ChatRoomMemberDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName));

        CreateMap<SendMessageDto, Message>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false));
    }
}
