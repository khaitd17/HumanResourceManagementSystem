using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("Payrolls")]
public class Payroll
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal KpiBonus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ResponsibilityAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LunchAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PhoneAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TravelAllowance { get; set; }

    public int StandardWorkingDays { get; set; }

    public int ActualWorkingDays { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InsuranceSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CompanyInsurance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EmployeeInsurance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PersonalIncomeTax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Approved, Paid

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}
