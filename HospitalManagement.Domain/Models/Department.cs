using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents a department in the hospital system
/// </summary>
public class Department
{
    /// <summary>
    /// The unique identifier of the department
    /// </summary>
    [Key]
    public int DeptNo { get; set; }

    /// <summary>
    /// The name of the department
    /// </summary>
    [Required]
    [StringLength(30)]
    public string DeptName { get; set; } = null!;

    /// <summary>
    /// The description of the department
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}