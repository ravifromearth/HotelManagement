using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents a doctor in the hospital system
/// </summary>
public class Doctor
{
    /// <summary>
    /// The unique identifier of the doctor (same as UserId)
    /// </summary>
    [Key]
    [ForeignKey("User")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the doctor
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The phone number of the doctor
    /// </summary>
    [StringLength(11)]
    public string? Phone { get; set; }

    /// <summary>
    /// The address of the doctor
    /// </summary>
    [StringLength(40)]
    public string? Address { get; set; }

    /// <summary>
    /// The birth date of the doctor
    /// </summary>
    [Required]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// The gender of the doctor (M/F)
    /// </summary>
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = null!;

    /// <summary>
    /// The department number the doctor belongs to
    /// </summary>
    [Required]
    [ForeignKey("Department")]
    public int DeptNo { get; set; }

    /// <summary>
    /// The charges per visit for the doctor
    /// </summary>
    [Required]
    public float ChargesPerVisit { get; set; }

    /// <summary>
    /// The monthly salary of the doctor
    /// </summary>
    public float? MonthlySalary { get; set; }

    /// <summary>
    /// The reputation index of the doctor
    /// </summary>
    public float? ReputeIndex { get; set; }

    /// <summary>
    /// The number of patients treated by the doctor
    /// </summary>
    [Required]
    public int PatientsTreated { get; set; } = 0;

    /// <summary>
    /// The qualifications of the doctor
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Qualification { get; set; } = null!;

    /// <summary>
    /// The specialization of the doctor
    /// </summary>
    [StringLength(100)]
    public string? Specialization { get; set; }

    /// <summary>
    /// The work experience of the doctor in years
    /// </summary>
    public int? WorkExperience { get; set; }

    /// <summary>
    /// The status of the doctor (1 = Present, 0 = Left)
    /// </summary>
    [Required]
    public int Status { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}