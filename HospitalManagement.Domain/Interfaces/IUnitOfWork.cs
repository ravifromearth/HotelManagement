namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Unit of work interface to manage transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Patient repository
    /// </summary>
    IPatientRepository Patients { get; }

    /// <summary>
    /// Doctor repository
    /// </summary>
    IDoctorRepository Doctors { get; }

    /// <summary>
    /// Department repository
    /// </summary>
    IDepartmentRepository Departments { get; }

    /// <summary>
    /// Appointment repository
    /// </summary>
    IAppointmentRepository Appointments { get; }

    /// <summary>
    /// OtherStaff repository
    /// </summary>
    IOtherStaffRepository OtherStaff { get; }

    /// <summary>
    /// Save changes to the database
    /// </summary>
    Task<int> SaveChangesAsync();
}