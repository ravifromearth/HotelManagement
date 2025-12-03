using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of Patient repository
/// </summary>
public class PatientRepository : Repository<Patient>, IPatientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PatientRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    /// <inheritdoc />
    public async Task<Patient?> GetPatientProfileAsync(int id)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Appointment?> GetCurrentAppointmentAsync(int patientId)
    {
        return await _dbContext.Appointments
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.Department)
            .Where(a => a.PatientId == patientId &&
                      (a.AppointmentStatus == 1 || a.AppointmentStatus == 2)) // 1=Approved, 2=Pending
            .OrderBy(a => a.Date)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetBillHistoryAsync(int patientId)
    {
        return await _dbContext.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId && a.BillAmount > 0)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId)
    {
        return await _dbContext.Appointments
            .Include(a => a.Doctor)
            .ThenInclude(d => d!.Department)
            .Where(a => a.PatientId == patientId && a.AppointmentStatus == 3) // 3=Completed
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }
}