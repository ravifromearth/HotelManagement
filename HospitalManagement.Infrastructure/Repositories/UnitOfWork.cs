using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of Unit of Work pattern
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IUserRepository? _userRepository;
    private IPatientRepository? _patientRepository;
    private IDoctorRepository? _doctorRepository;
    private IDepartmentRepository? _departmentRepository;
    private IAppointmentRepository? _appointmentRepository;
    private IOtherStaffRepository? _otherStaffRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);

    /// <inheritdoc />
    public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_context);

    /// <inheritdoc />
    public IDoctorRepository Doctors => _doctorRepository ??= new DoctorRepository(_context);

    /// <inheritdoc />
    public IDepartmentRepository Departments => _departmentRepository ??= new DepartmentRepository(_context);

    /// <inheritdoc />
    public IAppointmentRepository Appointments => _appointmentRepository ??= new AppointmentRepository(_context);

    /// <inheritdoc />
    public IOtherStaffRepository OtherStaff => _otherStaffRepository ??= new OtherStaffRepository(_context);

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}