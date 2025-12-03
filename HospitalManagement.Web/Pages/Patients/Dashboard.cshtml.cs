using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Patients
{
    [Authorize(Policy = "RequirePatientRole")]
    public class DashboardModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(IUnitOfWork unitOfWork, ILogger<DashboardModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Patient Patient { get; set; } = null!;
        public Appointment? CurrentAppointment { get; set; }
        public IEnumerable<Appointment> RecentAppointments { get; set; } = Enumerable.Empty<Appointment>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Verify current user is accessing their own dashboard
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Get patient profile
            Patient = await _unitOfWork.Patients.GetPatientProfileAsync(id) ??
                throw new InvalidOperationException("Patient not found");

            // Get current appointment
            CurrentAppointment = await _unitOfWork.Patients.GetCurrentAppointmentAsync(id);

            // Get recent appointments (treatment history)
            RecentAppointments = (await _unitOfWork.Patients.GetTreatmentHistoryAsync(id)).Take(5);

            return Page();
        }
    }
}