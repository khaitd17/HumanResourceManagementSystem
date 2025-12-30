using FluentValidation;
using HRM.ServiceLayer.DTOs.Attendance;

namespace HRM.ServiceLayer.Validators.Attendance;

public class CheckInValidator : AbstractValidator<CheckInDto>
{
    public CheckInValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID is required");

        RuleFor(x => x.WorkDate)
            .NotEmpty().WithMessage("Work date is required")
            .Must(date => date.Date <= DateTime.Today)
            .WithMessage("Work date cannot be in the future");

        RuleFor(x => x.CheckInTime)
            .NotEmpty().WithMessage("Check-in time is required");
    }
}
