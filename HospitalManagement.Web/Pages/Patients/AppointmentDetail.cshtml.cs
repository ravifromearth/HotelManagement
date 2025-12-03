using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Patients
{
    [Authorize(Policy = "RequirePatientRole")]
    public class AppointmentDetailModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AppointmentDetailModel> _logger;

        public AppointmentDetailModel(IUnitOfWork unitOfWork, ILogger<AppointmentDetailModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Appointment Appointment { get; set; } = null!;
        public int PatientId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get appointment details
            Appointment = await _unitOfWork.Appointments.GetByIdAsync(id) ??
                throw new InvalidOperationException("Appointment not found");

            // Set patient ID for navigation
            PatientId = Appointment.PatientId;

            // Verify current user is accessing their own appointment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != PatientId.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Load related data
            if (Appointment.DoctorId.HasValue)
            {
                var doctor = await _unitOfWork.Doctors.GetDoctorProfileAsync(Appointment.DoctorId.Value);
                if (doctor != null)
                {
                    Appointment.Doctor = doctor;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId) ??
                throw new InvalidOperationException("Appointment not found");

            // Verify current user is accessing their own appointment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != appointment.PatientId.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Check if appointment can be canceled (pending or approved only)
            if (appointment.AppointmentStatus != 1 && appointment.AppointmentStatus != 2)
            {
                TempData["ErrorMessage"] = "Only pending or approved appointments can be canceled.";
                return RedirectToPage(new { id = appointmentId });
            }

            try
            {
                // Cancel the appointment by setting status to rejected
                await _unitOfWork.Appointments.RejectAppointmentAsync(appointmentId);

                _logger.LogInformation("Appointment {AppointmentId} canceled by patient {PatientId}",
                    appointmentId, appointment.PatientId);

                TempData["SuccessMessage"] = "Your appointment has been canceled successfully.";
                return RedirectToPage("./Dashboard", new { id = appointment.PatientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling appointment {AppointmentId}", appointmentId);
                TempData["ErrorMessage"] = "An error occurred while canceling your appointment. Please try again.";
                return RedirectToPage(new { id = appointmentId });
            }
        }
    }
}