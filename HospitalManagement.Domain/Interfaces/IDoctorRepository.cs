using HospitalManagement.Domain.Models;

namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Doctor repository interface
/// </summary>
public interface IDoctorRepository : IRepository<Doctor>
{
    /// <summary>
    /// Get doctor profile details
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <returns>Doctor entity with associated user</returns>
    Task<Doctor?> GetDoctorProfileAsync(int id);

    /// <summary>
    /// Get doctors by department
    /// </summary>
    /// <param name="departmentName">Department name</param>
    /// <returns>List of doctors in the department</returns>
    Task<IEnumerable<Doctor>> GetByDepartmentAsync(string departmentName);

    /// <summary>
    /// Search doctors by name
    /// </summary>
    /// <param name="name">Search query for doctor name</param>
    /// <returns>List of matching doctors</returns>
    Task<IEnumerable<Doctor>> SearchByNameAsync(string name);

    /// <summary>
    /// Get pending appointments for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>List of pending appointments</returns>
    Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId);

    /// <summary>
    /// Get today's appointments for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>List of today's appointments</returns>
    Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId);

    /// <summary>
    /// Get patient history for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>List of completed appointments</returns>
    Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId);
}