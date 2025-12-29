using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("Attendances")]
public class Attendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime WorkDate { get; set; }

    public TimeSpan? CheckIn { get; set; }

    public TimeSpan? CheckOut { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TotalHours { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Present"; // Present, Late, Absent, Leave

    public int? LateMinutes { get; set; }

    public int? EarlyLeaveMinutes { get; set; }

    public int? OvertimeHours { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? CheckInLocation { get; set; } // GPS/IP tracking

    [MaxLength(100)]
    public string? CheckOutLocation { get; set; } // GPS/IP tracking

    // Navigation Properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
