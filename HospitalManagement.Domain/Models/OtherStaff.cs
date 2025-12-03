using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents non-medical staff in the hospital system
/// </summary>
public class OtherStaff
{
    /// <summary>
    /// The unique identifier of the staff member
    /// </summary>
    [Key]
    public int StaffId { get; set; }

    /// <summary>
    /// The name of the staff member
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The phone number of the staff member
    /// </summary>
    [StringLength(11)]
    public string? Phone { get; set; }

    /// <summary>
    /// The address of the staff member
    /// </summary>
    [StringLength(30)]
    public string? Address { get; set; }

    /// <summary>
    /// The designation of the staff member
    /// </summary>
    [Required]
    [StringLength(15)]
    public string Designation { get; set; } = null!;

    /// <summary>
    /// The gender of the staff member (M/F)
    /// </summary>
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = null!;

    /// <summary>
    /// The birth date of the staff member
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// The highest qualification of the staff member
    /// </summary>
    [StringLength(50)]
    public string? HighestQualification { get; set; }

    /// <summary>
    /// The salary of the staff member
    /// </summary>
    public float? Salary { get; set; }
}