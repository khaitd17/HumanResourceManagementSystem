using HRM.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRM.DataLayer.Data;

public class HRMDbContext : DbContext
{
    public HRMDbContext(DbContextOptions<HRMDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }
    public DbSet<PayrollConfig> PayrollConfigs { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatRoomMember> ChatRoomMembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.EmployeeId).IsUnique();
        });

        // Employee Configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.EmployeeCode).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Attendance Configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.WorkDate }).IsUnique();
            
            entity.HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // LeaveRequest Configuration
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payroll Configuration
        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.Month, e.Year }).IsUnique();
            
            entity.HasOne(p => p.Employee)
                .WithMany(e => e.Payrolls)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ChatRoomMember Configuration
        modelBuilder.Entity<ChatRoomMember>(entity =>
        {
            entity.HasIndex(e => new { e.ChatRoomId, e.EmployeeId }).IsUnique();
            
            entity.HasOne(crm => crm.ChatRoom)
                .WithMany(cr => cr.ChatRoomMembers)
                .HasForeignKey(crm => crm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(crm => crm.Employee)
                .WithMany(e => e.ChatRoomMembers)
                .HasForeignKey(crm => crm.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Message Configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasOne(m => m.ChatRoom)
                .WithMany(cr => cr.Messages)
                .HasForeignKey(m => m.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                .WithMany(e => e.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", Description = "Information Technology", CreatedAt = DateTime.UtcNow },
            new Department { Id = 2, Name = "HR", Description = "Human Resources", CreatedAt = DateTime.UtcNow },
            new Department { Id = 3, Name = "Finance", Description = "Finance & Accounting", CreatedAt = DateTime.UtcNow },
            new Department { Id = 4, Name = "Sales", Description = "Sales & Marketing", CreatedAt = DateTime.UtcNow }
        );

        // Seed PayrollConfig
        modelBuilder.Entity<PayrollConfig>().HasData(
            new PayrollConfig
            {
                Id = 1,
                StandardWorkingDays = 22,
                PersonalTaxDeduction = 11000000,
                CompanyInsuranceRate = 17.5m,
                EmployeeInsuranceRate = 10.5m,
                EffectiveFrom = new DateTime(2024, 1, 1),
                IsActive = true
            }
        );
    }
}
