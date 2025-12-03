using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Appointment repository interface
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Get available appointment slots for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of available appointment slots</returns>
    Task<IEnumerable<DateTime>> GetFreeSlotsAsync(int doctorId, int patientId);

    /// <summary>
    /// Create a new appointment request
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <param name="slotTime">Appointment slot time</param>
    /// <returns>New appointment entity</returns>
    Task<Appointment> CreateAppointmentAsync(int doctorId, int patientId, DateTime slotTime);

    /// <summary>
    /// Approve an appointment request
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> ApproveAppointmentAsync(int appointmentId);

    /// <summary>
    /// Reject an appointment request
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> RejectAppointmentAsync(int appointmentId);

    /// <summary>
    /// Complete an appointment and update treatment details
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="disease">Diagnosed disease</param>
    /// <param name="progress">Treatment progress</param>
    /// <param name="prescription">Prescribed medication</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> CompleteTreatmentAsync(int appointmentId, string disease, string progress, string prescription);

    /// <summary>
    /// Generate and update bill for an appointment
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <param name="amount">Bill amount</param>
    /// <param name="isPaid">Whether the bill is paid</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> GenerateBillAsync(int appointmentId, float amount, bool isPaid);

    /// <summary>
    /// Check if there are notifications for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of notifications</returns>
    Task<IEnumerable<Appointment>> GetPatientNotificationsAsync(int patientId);

    /// <summary>
    /// Update feedback status for an appointment
    /// </summary>
    /// <param name="appointmentId">Appointment ID</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> UpdateFeedbackStatusAsync(int appointmentId);
}