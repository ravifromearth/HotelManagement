using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents a patient in the hospital system
/// </summary>
public class Patient
{
    /// <summary>
    /// The unique identifier of the patient (same as UserId)
    /// </summary>
    [Key]
    [ForeignKey("User")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the patient
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The phone number of the patient
    /// </summary>
    [StringLength(11)]
    public string? Phone { get; set; }

    /// <summary>
    /// The address of the patient
    /// </summary>
    [StringLength(40)]
    public string? Address { get; set; }

    /// <summary>
    /// The birth date of the patient
    /// </summary>
    [Required]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// The gender of the patient (M/F)
    /// </summary>
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = null!;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}