using FluentValidation;
using HRM.ServiceLayer.DTOs.LeaveRequest;

namespace HRM.ServiceLayer.Validators.LeaveRequest;

public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequestDto>
{
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID is required");

        RuleFor(x => x.FromDate)
            .NotEmpty().WithMessage("From date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("From date cannot be in the past");

        RuleFor(x => x.ToDate)
            .NotEmpty().WithMessage("To date is required")
            .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("To date must be after or equal to from date");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");

        RuleFor(x => x.LeaveType)
            .NotEmpty().WithMessage("Leave type is required")
            .Must(type => new[] { "Annual", "Sick", "Personal", "Unpaid", "Other" }.Contains(type))
            .WithMessage("Invalid leave type");
    }
}
