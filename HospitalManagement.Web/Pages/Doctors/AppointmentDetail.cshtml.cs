using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Doctors
{
    [Authorize(Policy = "RequireDoctorRole")]
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
        public int DoctorId { get; set; }
        public IEnumerable<Appointment> PatientHistory { get; set; } = Enumerable.Empty<Appointment>();

        [BindProperty]
        public TreatmentInfoModel TreatmentInfo { get; set; } = new TreatmentInfoModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                TempData["ErrorMessage"] = "Appointment not found. It may have been cancelled or deleted.";
                return RedirectToPage("/Doctors/Dashboard", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }
            Appointment = appointment;

            // Set doctor ID for navigation
            DoctorId = Appointment.DoctorId ?? 0;

            // Verify current user is the assigned doctor
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != DoctorId.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Load related data - Patient
            var patient = await _unitOfWork.Patients.GetPatientProfileAsync(Appointment.PatientId);
            if (patient != null)
            {
                Appointment.Patient = patient;
            }

            // Get patient treatment history with this doctor
            if (DoctorId > 0)
            {
                PatientHistory = (await _unitOfWork.Patients.GetTreatmentHistoryAsync(Appointment.PatientId))
                    .Where(a => a.DoctorId == DoctorId && a.AppointId != id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found when approving", appointmentId);
                TempData["ErrorMessage"] = "Appointment not found. It may have been cancelled or deleted.";
                return RedirectToPage("/Doctors/Dashboard", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }

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
                return RedirectToPage(new { id = appointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving appointment {AppointmentId}", appointmentId);
                TempData["ErrorMessage"] = "An error occurred while approving the appointment. Please try again.";
                return RedirectToPage(new { id = appointmentId });
            }
        }

        public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
        {
            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found when rejecting", appointmentId);
                TempData["ErrorMessage"] = "Appointment not found. It may have been cancelled or deleted.";
                return RedirectToPage("/Doctors/Dashboard", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }

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
                return RedirectToPage("./Dashboard", new { id = appointment.DoctorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", appointmentId);
                TempData["ErrorMessage"] = "An error occurred while rejecting the appointment. Please try again.";
                return RedirectToPage(new { id = appointmentId });
            }
        }

        public async Task<IActionResult> OnPostCompleteTreatmentAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TreatmentInfo.AppointmentId);
            }

            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(TreatmentInfo.AppointmentId) ??
                throw new InvalidOperationException("Appointment not found");

            // Verify current user is the assigned doctor
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (appointment.DoctorId.ToString() != userId)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            try
            {
                // Update treatment details
                await _unitOfWork.Appointments.CompleteTreatmentAsync(
                    TreatmentInfo.AppointmentId,
                    TreatmentInfo.Disease,
                    TreatmentInfo.Progress,
                    TreatmentInfo.Prescription);

                // Generate bill
                await _unitOfWork.Appointments.GenerateBillAsync(
                    TreatmentInfo.AppointmentId,
                    TreatmentInfo.BillAmount,
                    TreatmentInfo.IsPaid);

                _logger.LogInformation("Treatment completed for appointment {AppointmentId} by doctor {DoctorId}",
                    TreatmentInfo.AppointmentId, appointment.DoctorId);

                TempData["SuccessMessage"] = "Treatment has been completed successfully.";
                return RedirectToPage(new { id = TreatmentInfo.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing treatment for appointment {AppointmentId}", TreatmentInfo.AppointmentId);
                TempData["ErrorMessage"] = "An error occurred while completing the treatment. Please try again.";
                return RedirectToPage(new { id = TreatmentInfo.AppointmentId });
            }
        }

        public async Task<IActionResult> OnPostUpdateTreatmentAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TreatmentInfo.AppointmentId);
            }

            // Get appointment details
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(TreatmentInfo.AppointmentId) ??
                throw new InvalidOperationException("Appointment not found");

            // Verify current user is the assigned doctor
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (appointment.DoctorId.ToString() != userId)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            try
            {
                // Update treatment details
                await _unitOfWork.Appointments.CompleteTreatmentAsync(
                    TreatmentInfo.AppointmentId,
                    TreatmentInfo.Disease,
                    TreatmentInfo.Progress,
                    TreatmentInfo.Prescription);

                // Generate or update bill
                await _unitOfWork.Appointments.GenerateBillAsync(
                    TreatmentInfo.AppointmentId,
                    TreatmentInfo.BillAmount,
                    TreatmentInfo.IsPaid);

                _logger.LogInformation("Treatment updated for appointment {AppointmentId} by doctor {DoctorId}",
                    TreatmentInfo.AppointmentId, appointment.DoctorId);

                TempData["SuccessMessage"] = "Treatment information has been updated successfully.";
                return RedirectToPage(new { id = TreatmentInfo.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating treatment for appointment {AppointmentId}", TreatmentInfo.AppointmentId);
                TempData["ErrorMessage"] = "An error occurred while updating the treatment. Please try again.";
                return RedirectToPage(new { id = TreatmentInfo.AppointmentId });
            }
        }
    }

    public class TreatmentInfoModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please provide a diagnosis")]
        [StringLength(100)]
        public string Disease { get; set; } = string.Empty;

        [StringLength(100)]
        public string Progress { get; set; } = string.Empty;

        [StringLength(100)]
        public string Prescription { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "Bill amount must be between $0 and $10,000")]
        public float BillAmount { get; set; }

        [Display(Name = "Bill Paid")]
        public bool IsPaid { get; set; }
    }
}