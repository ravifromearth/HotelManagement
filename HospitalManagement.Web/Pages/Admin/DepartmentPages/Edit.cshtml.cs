using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.Pages.Admin.DepartmentPages
{
    [Authorize(Policy = "RequireAdminRole")]
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IUnitOfWork unitOfWork, ILogger<EditModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public DepartmentEditViewModel DepartmentModel { get; set; } = new DepartmentEditViewModel();

        public class DepartmentEditViewModel
        {
            public int DeptNo { get; set; }

            [Required(ErrorMessage = "Department name is required")]
            [StringLength(30, ErrorMessage = "Department name cannot exceed 30 characters")]
            [Display(Name = "Department Name")]
            public string DeptName { get; set; } = string.Empty;

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            [Display(Name = "Description")]
            public string? Description { get; set; }

            // Doctor count is for display only
            [Display(Name = "Number of Doctors")]
            public int DoctorCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var departmentsWithDoctors = await _unitOfWork.Departments.GetAllWithDoctorsAsync();
                var department = departmentsWithDoctors.FirstOrDefault(d => d.DeptNo == id);

                if (department == null)
                {
                    _logger.LogWarning("Department with ID {DepartmentId} not found", id);
                    TempData["ErrorMessage"] = "Department not found.";
                    return RedirectToPage("/Admin/Departments");
                }

                DepartmentModel = new DepartmentEditViewModel
                {
                    DeptNo = department.DeptNo,
                    DeptName = department.DeptName,
                    Description = department.Description,
                    DoctorCount = department.Doctors.Count
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department {DepartmentId} for editing", id);
                TempData["ErrorMessage"] = "Error retrieving department information. Please try again.";
                return RedirectToPage("/Admin/Departments");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(DepartmentModel.DeptNo);
                if (department == null)
                {
                    _logger.LogWarning("Department {DepartmentId} not found during update", DepartmentModel.DeptNo);
                    TempData["ErrorMessage"] = "Department not found.";
                    return RedirectToPage("/Admin/Departments");
                }

                // Check if the department name already exists (excluding current department)
                var existingDepartment = await _unitOfWork.Departments.GetByNameAsync(DepartmentModel.DeptName);
                if (existingDepartment != null && existingDepartment.DeptNo != DepartmentModel.DeptNo)
                {
                    ModelState.AddModelError("DepartmentModel.DeptName", "A department with this name already exists.");
                    return Page();
                }

                // Update department properties
                department.DeptName = DepartmentModel.DeptName;
                department.Description = DepartmentModel.Description;

                // Update in the database
                _unitOfWork.Departments.Update(department);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Department {DepartmentName} (ID: {DepartmentId}) updated successfully",
                    department.DeptName, department.DeptNo);
                TempData["SuccessMessage"] = "Department updated successfully.";
                return RedirectToPage("/Admin/Departments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {DepartmentId}", DepartmentModel.DeptNo);
                TempData["ErrorMessage"] = "Error updating department. Please try again.";
                return Page();
            }
        }
    }
}