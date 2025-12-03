using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.Pages.Admin.DepartmentPages
{
    [Authorize(Policy = "RequireAdminRole")]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IUnitOfWork unitOfWork, ILogger<DeleteModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public DepartmentDeleteViewModel DepartmentModel { get; set; } = new DepartmentDeleteViewModel();

        public class DepartmentDeleteViewModel
        {
            public int DeptNo { get; set; }

            [Display(Name = "Department Name")]
            public string DeptName { get; set; } = string.Empty;

            [Display(Name = "Description")]
            public string? Description { get; set; }

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

                DepartmentModel = new DepartmentDeleteViewModel
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
                _logger.LogError(ex, "Error retrieving department {DepartmentId} for deletion", id);
                TempData["ErrorMessage"] = "Error retrieving department information. Please try again.";
                return RedirectToPage("/Admin/Departments");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(DepartmentModel.DeptNo);
                if (department == null)
                {
                    _logger.LogWarning("Department {DepartmentId} not found during delete confirmation", DepartmentModel.DeptNo);
                    TempData["ErrorMessage"] = "Department not found.";
                    return RedirectToPage("/Admin/Departments");
                }

                // Check if there are doctors assigned to this department
                var departmentWithDoctors = await _unitOfWork.Departments.GetAllWithDoctorsAsync();
                var departmentToDelete = departmentWithDoctors.FirstOrDefault(d => d.DeptNo == DepartmentModel.DeptNo);
                if (departmentToDelete != null && departmentToDelete.Doctors.Any())
                {
                    _logger.LogWarning("Cannot delete department {DepartmentName} (ID: {DepartmentId}) as it has {DoctorCount} doctors assigned",
                        department.DeptName, DepartmentModel.DeptNo, departmentToDelete.Doctors.Count);
                    TempData["ErrorMessage"] = "Cannot delete department as it has doctors assigned. Reassign doctors first.";
                    return RedirectToPage("/Admin/Departments");
                }

                _unitOfWork.Departments.Delete(department);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Department {DepartmentName} (ID: {DepartmentId}) deleted successfully",
                    department.DeptName, department.DeptNo);
                TempData["SuccessMessage"] = "Department deleted successfully.";
                return RedirectToPage("/Admin/Departments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId}", DepartmentModel.DeptNo);
                TempData["ErrorMessage"] = "Error deleting department. Please try again.";
                return RedirectToPage("/Admin/Departments");
            }
        }
    }
}