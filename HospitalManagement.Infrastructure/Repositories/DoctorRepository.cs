using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of Doctor repository
/// </summary>
public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    /// <inheritdoc />
    public async Task<Doctor?> GetDoctorProfileAsync(int id)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string departmentName)
    {
        return await _dbSet
            .Include(d => d.Department)
            .Where(d => d.Department != null && d.Department.DeptName == departmentName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Doctor>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Include(d => d.Department)
            .Where(d => d.Name.Contains(name))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId)
    {
        return await _dbContext.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == 2) // 2=Pending
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId)
    {
        DateTime today = DateTime.Today;
        return await _dbContext.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId &&
                      a.Date != null &&
                      a.Date.Value.Date == today &&
                      a.AppointmentStatus == 1) // 1=Approved
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId)
    {
        return await _dbContext.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == 3) // 3=Completed
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }
}