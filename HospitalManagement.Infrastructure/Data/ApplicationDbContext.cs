using HospitalManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Data;

/// <summary>
/// Entity Framework database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<OtherStaff> OtherStaff { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names
        modelBuilder.Entity<User>().ToTable("LoginTable");
        modelBuilder.Entity<Patient>().ToTable("Patient");
        modelBuilder.Entity<Doctor>().ToTable("Doctor");
        modelBuilder.Entity<Department>().ToTable("Department");
        modelBuilder.Entity<Appointment>().ToTable("Appointment");
        modelBuilder.Entity<OtherStaff>().ToTable("OtherStaff");

        // Primary keys and field mappings
        modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("LoginID");
        modelBuilder.Entity<Patient>().Property(p => p.Id).HasColumnName("PatientID");
        modelBuilder.Entity<Doctor>().Property(d => d.Id).HasColumnName("DoctorID");
        modelBuilder.Entity<Appointment>().Property(a => a.AppointId).HasColumnName("AppointID");
        modelBuilder.Entity<OtherStaff>().Property(s => s.StaffId).HasColumnName("StaffID");

        // Model configuration for Doctor
        modelBuilder.Entity<Doctor>()
            .Property(d => d.ChargesPerVisit)
            .HasColumnName("Charges_Per_Visit");

        modelBuilder.Entity<Doctor>()
            .Property(d => d.PatientsTreated)
            .HasColumnName("Patients_Treated");

        modelBuilder.Entity<Doctor>()
            .Property(d => d.WorkExperience)
            .HasColumnName("Work_Experience");

        // Model configuration for Appointment
        modelBuilder.Entity<Appointment>()
            .Property(a => a.AppointmentStatus)
            .HasColumnName("Appointment_Status");

        modelBuilder.Entity<Appointment>()
            .Property(a => a.BillAmount)
            .HasColumnName("Bill_Amount");

        modelBuilder.Entity<Appointment>()
            .Property(a => a.BillStatus)
            .HasColumnName("Bill_Status");

        // Model configuration for OtherStaff
        modelBuilder.Entity<OtherStaff>()
            .Property(s => s.HighestQualification)
            .HasColumnName("Highest_Qualification");

        // Relationships
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.Id);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.Doctor)
            .HasForeignKey<Doctor>(d => d.Id);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Department)
            .WithMany(dept => dept.Doctors)
            .HasForeignKey(d => d.DeptNo);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}