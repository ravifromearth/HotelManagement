using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Domain.Models;

/// <summary>
/// Represents an appointment between a doctor and a patient
/// </summary>
public class Appointment
{
    /// <summary>
    /// The unique identifier of the appointment
    /// </summary>
    [Key]
    public int AppointId { get; set; }

    /// <summary>
    /// The doctor ID associated with the appointment
    /// </summary>
    [ForeignKey("Doctor")]
    public int? DoctorId { get; set; }

    /// <summary>
    /// The patient ID associated with the appointment
    /// </summary>
    [ForeignKey("Patient")]
    public int PatientId { get; set; }

    /// <summary>
    /// The date and time of the appointment
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// The status of the appointment
    /// 1 - Approved
    /// 2 - Pending
    /// 3 - Completed
    /// 4 - Rejected
    /// </summary>
    public int? AppointmentStatus { get; set; }

    /// <summary>
    /// The bill amount for the appointment
    /// </summary>
    public float? BillAmount { get; set; }

    /// <summary>
    /// The status of the bill (Paid/Unpaid)
    /// </summary>
    [StringLength(10)]
    public string? BillStatus { get; set; }

    /// <summary>
    /// The doctor notification status
    /// 1 - Seen
    /// 2 - Unseen
    /// </summary>
    public int? DoctorNotification { get; set; }

    /// <summary>
    /// The patient notification status
    /// 1 - Seen
    /// 2 - Unseen
    /// </summary>
    public int? PatientNotification { get; set; }

    /// <summary>
    /// The feedback status
    /// 1 - Given
    /// 2 - Pending
    /// </summary>
    public int? FeedbackStatus { get; set; }

    /// <summary>
    /// The disease diagnosed during the appointment
    /// </summary>
    [StringLength(100)]
    public string? Disease { get; set; }

    /// <summary>
    /// The progress of the treatment
    /// </summary>
    [StringLength(100)]
    public string? Progress { get; set; }

    /// <summary>
    /// The prescription given during the appointment
    /// </summary>
    [StringLength(100)]
    public string? Prescription { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient Patient { get; set; } = null!;
}