using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Patients
{
    [Authorize(Policy = "RequirePatientRole")]
    public class BookAppointmentModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookAppointmentModel> _logger;

        public BookAppointmentModel(IUnitOfWork unitOfWork, ILogger<BookAppointmentModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public AppointmentRequestModel AppointmentRequest { get; set; } = new AppointmentRequestModel();

        public int PatientId { get; set; }
        public IEnumerable<Department> Departments { get; set; } = Enumerable.Empty<Department>();
        public IEnumerable<Doctor> Doctors { get; set; } = Enumerable.Empty<Doctor>();
        public Department? SelectedDepartment { get; set; }
        public Doctor? SelectedDoctor { get; set; }
        public IEnumerable<DateTime> AvailableSlots { get; set; } = Enumerable.Empty<DateTime>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Verify current user is accessing their own appointment booking
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            PatientId = id;

            // Load all departments
            Departments = await _unitOfWork.Departments.GetAllAsync();

            // If there's a department selected (via query string)
            if (Request.Query.TryGetValue("AppointmentRequest.DepartmentId", out var deptIdValues) &&
                int.TryParse(deptIdValues.FirstOrDefault(), out int departmentId))
            {
                AppointmentRequest.DepartmentId = departmentId;
                SelectedDepartment = await _unitOfWork.Departments.GetByIdAsync(departmentId);
                Doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(SelectedDepartment?.DeptName ?? "");
            }

            // If there's a doctor selected (via query string)
            if (Request.Query.TryGetValue("AppointmentRequest.DoctorId", out var docIdValues) &&
                int.TryParse(docIdValues.FirstOrDefault(), out int doctorId))
            {
                AppointmentRequest.DoctorId = doctorId;
                SelectedDoctor = await _unitOfWork.Doctors.GetDoctorProfileAsync(doctorId);

                // Get date from query string if it exists
                if (Request.Query.TryGetValue("AppointmentRequest.SlotDate", out var dateValues) &&
                    DateTime.TryParse(dateValues.FirstOrDefault(), out DateTime slotDate))
                {
                    AppointmentRequest.SlotDate = slotDate;

                    // Get available slots for the selected doctor and date
                    var allSlots = await _unitOfWork.Appointments.GetFreeSlotsAsync(doctorId, PatientId);
                    AvailableSlots = allSlots.Where(s => s.Date == slotDate.Date);
                }
                else
                {
                    // Default to today + 1 for slot date
                    AppointmentRequest.SlotDate = DateTime.Today.AddDays(1);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (AppointmentRequest.DepartmentId.HasValue && !AppointmentRequest.DoctorId.HasValue)
            {
                // Department was selected, redirect to same page to show doctors
                return RedirectToPage(new { id, AppointmentRequest.DepartmentId });
            }

            if (AppointmentRequest.DoctorId.HasValue && AppointmentRequest.SlotDate.HasValue &&
                string.IsNullOrEmpty(AppointmentRequest.SlotTime))
            {
                // Doctor was selected, redirect to same page to show available slots
                return RedirectToPage(new { id, AppointmentRequest.DepartmentId, AppointmentRequest.DoctorId, AppointmentRequest.SlotDate });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostBookAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            // Verify patient id matches current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Create appointment date/time from selected date and time
            var slotDate = AppointmentRequest.SlotDate!.Value;
            var timeComponents = AppointmentRequest.SlotTime!.Split(':');
            var hours = int.Parse(timeComponents[0]);
            var minutes = int.Parse(timeComponents[1]);
            var appointmentDateTime = new DateTime(
                slotDate.Year, slotDate.Month, slotDate.Day, hours, minutes, 0);

            // Create the appointment
            try
            {
                var appointment = await _unitOfWork.Appointments.CreateAppointmentAsync(
                    AppointmentRequest.DoctorId!.Value,
                    id,
                    appointmentDateTime);

                _logger.LogInformation("Appointment created successfully for patient {PatientId} with doctor {DoctorId}",
                    id, AppointmentRequest.DoctorId);

                TempData["SuccessMessage"] = "Your appointment request has been submitted and is awaiting approval.";
                return RedirectToPage("./Dashboard", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for patient {PatientId}", id);
                ModelState.AddModelError("", "An error occurred while booking your appointment. Please try again.");
                return await OnGetAsync(id);
            }
        }
    }

    public class AppointmentRequestModel
    {
        public int PatientId { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Doctor")]
        public int? DoctorId { get; set; }

        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime? SlotDate { get; set; }

        [Display(Name = "Time Slot")]
        public string? SlotTime { get; set; }
    }
}