using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Doctors
{
    [Authorize(Policy = "RequireDoctorRole")]
    public class DashboardModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(IUnitOfWork unitOfWork, ILogger<DashboardModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Doctor Doctor { get; set; } = null!;
        public IEnumerable<Appointment> TodaysAppointments { get; set; } = Enumerable.Empty<Appointment>();
        public IEnumerable<Appointment> PendingAppointments { get; set; } = Enumerable.Empty<Appointment>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Verify current user is accessing their own dashboard
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Get doctor profile
            var doctor = await _unitOfWork.Doctors.GetDoctorProfileAsync(id);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found", id);
                TempData["ErrorMessage"] = "Doctor profile not found. Please contact administrator.";
                return RedirectToPage("/Index");
            }
            Doctor = doctor;

            // Get today's appointments
            TodaysAppointments = await _unitOfWork.Doctors.GetTodaysAppointmentsAsync(id);

            // Get pending appointment requests
            PendingAppointments = await _unitOfWork.Doctors.GetPendingAppointmentsAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId) ??
                throw new InvalidOperationException("Appointment not found");

            // Verify current user is the assigned doctor
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (appointment.DoctorId.ToString() != userId)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            try
            {
                // Approve the appointment
                await _unitOfWork.Appointments.ApproveAppointmentAsync(appointmentId);

                _logger.LogInformation("Appointment {AppointmentId} approved by doctor {DoctorId}",
                    appointmentId, appointment.DoctorId);

                TempData["SuccessMessage"] = "Appointment has been approved successfully.";
                return RedirectToPage(new { id = appointment.DoctorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving appointment {AppointmentId}", appointmentId);
                TempData["ErrorMessage"] = "An error occurred while approving the appointment. Please try again.";
                return RedirectToPage(new { id = appointment.DoctorId });
            }
        }

        public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId) ??
                throw new InvalidOperationException("Appointment not found");

            // Verify current user is the assigned doctor
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (appointment.DoctorId.ToString() != userId)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            try
            {
                // Reject the appointment
                await _unitOfWork.Appointments.RejectAppointmentAsync(appointmentId);

                _logger.LogInformation("Appointment {AppointmentId} rejected by doctor {DoctorId}",
                    appointmentId, appointment.DoctorId);

                TempData["SuccessMessage"] = "Appointment has been rejected.";
                return RedirectToPage(new { id = appointment.DoctorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", appointmentId);
                TempData["ErrorMessage"] = "An error occurred while rejecting the appointment. Please try again.";
                return RedirectToPage(new { id = appointment.DoctorId });
            }
        }
    }
}