using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The user's password
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// The user's email address
    /// </summary>
    [Required]
    [StringLength(30)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// The type of user
    /// 1 - Patient
    /// 2 - Doctor
    /// 3 - Admin
    /// </summary>
    [Required]
    public int Type { get; set; }

    // Navigation properties
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}