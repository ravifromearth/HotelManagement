using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Patient repository interface
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    /// <summary>
    /// Get patient profile details
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <returns>Patient entity with associated user</returns>
    Task<Patient?> GetPatientProfileAsync(int id);

    /// <summary>
    /// Search patients by name
    /// </summary>
    /// <param name="name">Search query for patient name</param>
    /// <returns>List of matching patients</returns>
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);

    /// <summary>
    /// Get current appointment for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>Current appointment if exists</returns>
    Task<Appointment?> GetCurrentAppointmentAsync(int patientId);

    /// <summary>
    /// Get bill history for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of appointments with bills</returns>
    Task<IEnumerable<Appointment>> GetBillHistoryAsync(int patientId);

    /// <summary>
    /// Get treatment history for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of completed appointments</returns>
    Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId);
}