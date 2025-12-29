using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("Employees")]
public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public int? DepartmentId { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    public DateTime? JoinDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? EmergencyContact { get; set; }

    [MaxLength(20)]
    public string? EmergencyPhone { get; set; }

    [MaxLength(50)]
    public string? IdentityCard { get; set; }

    [MaxLength(20)]
    public string? TaxCode { get; set; }

    [MaxLength(50)]
    public string? BankAccount { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    public DateTime? ResignDate { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? UpdatedBy { get; set; }

    // Navigation Properties
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
