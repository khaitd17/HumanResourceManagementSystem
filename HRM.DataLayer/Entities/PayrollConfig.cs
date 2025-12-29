using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.DataLayer.Entities;

[Table("PayrollConfigs")]
public class PayrollConfig
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int StandardWorkingDays { get; set; } = 22;

    [Column(TypeName = "decimal(5,2)")]
    public decimal PersonalTaxDeduction { get; set; } = 11000000; // VND

    [Column(TypeName = "decimal(5,2)")]
    public decimal CompanyInsuranceRate { get; set; } = 17.5m; // %

    [Column(TypeName = "decimal(5,2)")]
    public decimal EmployeeInsuranceRate { get; set; } = 10.5m; // %

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;
}
