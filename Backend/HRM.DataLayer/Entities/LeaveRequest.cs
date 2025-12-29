using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("LeaveRequests")]
public class LeaveRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(50)]
    public string LeaveType { get; set; } = "Annual"; // Annual, Sick, Personal, etc.

    [Column(TypeName = "decimal(5,2)")]
    public decimal TotalDays { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
