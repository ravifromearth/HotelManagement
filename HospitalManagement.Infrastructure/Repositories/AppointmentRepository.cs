using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

/// <summary>
/// Implementation of Appointment repository
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DateTime>> GetFreeSlotsAsync(int doctorId, int patientId)
    {
        // Get the doctor's working hours (assuming 9 AM to 5 PM with 1-hour slots)
        var startHour = 9;
        var endHour = 17;
        var slotDuration = TimeSpan.FromHours(1);

        // Get the booked slots for the doctor in the next 7 days
        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(7);

        var bookedSlots = await _dbContext.Appointments
            .Where(a => a.DoctorId == doctorId &&
                      a.Date >= startDate &&
                      a.Date < endDate &&
                      (a.AppointmentStatus == 1 || a.AppointmentStatus == 2)) // 1=Approved, 2=Pending
            .Select(a => a.Date)
            .ToListAsync();

        // Generate all possible slots
        var allSlots = new List<DateTime>();
        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            for (var hour = startHour; hour < endHour; hour++)
            {
                var slot = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                if (slot > DateTime.Now) // Only future slots
                {
                    allSlots.Add(slot);
                }
            }
        }

        // Filter out booked slots
        return allSlots.Where(slot => !bookedSlots.Any(booked =>
                                    booked != null &&
                                    booked.Value.Year == slot.Year &&
                                    booked.Value.Month == slot.Month &&
                                    booked.Value.Day == slot.Day &&
                                    booked.Value.Hour == slot.Hour));
    }

    /// <inheritdoc />
    public async Task<Appointment> CreateAppointmentAsync(int doctorId, int patientId, DateTime slotTime)
    {
        var appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            Date = slotTime,
            AppointmentStatus = 2, // 2=Pending
            BillAmount = 0,
            BillStatus = "Not Generated",
            FeedbackStatus = 2 // 2=Pending
        };

        await _dbSet.AddAsync(appointment);
        await _context.SaveChangesAsync();

        return appointment;
    }

    /// <inheritdoc />
    public async Task<bool> ApproveAppointmentAsync(int appointmentId)
    {
        var appointment = await _dbSet.FindAsync(appointmentId);
        if (appointment == null)
        {
            return false;
        }

        appointment.AppointmentStatus = 1; // 1=Approved
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RejectAppointmentAsync(int appointmentId)
    {
        var appointment = await _dbSet.FindAsync(appointmentId);
        if (appointment == null)
        {
            return false;
        }

        appointment.AppointmentStatus = 4; // 4=Rejected
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CompleteTreatmentAsync(int appointmentId, string disease, string progress, string prescription)
    {
        var appointment = await _dbSet.FindAsync(appointmentId);
        if (appointment == null)
        {
            return false;
        }

        appointment.Disease = disease;
        appointment.Progress = progress;
        appointment.Prescription = prescription;
        appointment.AppointmentStatus = 3; // 3=Completed
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> GenerateBillAsync(int appointmentId, float amount, bool isPaid)
    {
        var appointment = await _dbSet.FindAsync(appointmentId);
        if (appointment == null)
        {
            return false;
        }

        appointment.BillAmount = amount;
        appointment.BillStatus = isPaid ? "Paid" : "Pending";
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetPatientNotificationsAsync(int patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId &&
                      (a.AppointmentStatus == 1 || a.AppointmentStatus == 4) && // 1=Approved, 4=Rejected
                      a.Date >= DateTime.Today)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> UpdateFeedbackStatusAsync(int appointmentId)
    {
        var appointment = await _dbSet.FindAsync(appointmentId);
        if (appointment == null)
        {
            return false;
        }

        appointment.FeedbackStatus = 1; // 1=Given
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }
}