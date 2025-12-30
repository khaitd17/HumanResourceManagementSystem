using FluentValidation;
using HRM.ServiceLayer.DTOs.Employee;

namespace HRM.ServiceLayer.Validators.Employee;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required")
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9]{10,11}$").WithMessage("Phone must be 10-11 digits")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("Base salary must be greater than 0");

        RuleFor(x => x.JoinDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Join date cannot be in the future")
            .When(x => x.JoinDate.HasValue);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Employee must be at least 18 years old")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Gender)
            .Must(gender => gender == null || new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Gender must be Male, Female, or Other");
    }
}
