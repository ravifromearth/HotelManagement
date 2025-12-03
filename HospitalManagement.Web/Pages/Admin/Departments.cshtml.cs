using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.Web.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class DepartmentsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DepartmentsModel> _logger;

        public DepartmentsModel(IUnitOfWork unitOfWork, ILogger<DepartmentsModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IEnumerable<Department> Departments { get; set; } = Enumerable.Empty<Department>();

        public async Task OnGetAsync()
        {
            try
            {
                Departments = await _unitOfWork.Departments.GetAllWithDoctorsAsync();
                _logger.LogInformation("Retrieved {Count} departments", Departments.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                TempData["ErrorMessage"] = "Error retrieving departments. Please try again.";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                _logger.LogWarning("Department {DepartmentId} not found", id);
                TempData["ErrorMessage"] = "Department not found.";
                return RedirectToPage();
            }

            // Check if there are doctors assigned to this department
            var departmentWithDoctors = await _unitOfWork.Departments.GetAllWithDoctorsAsync();
            var departmentToDelete = departmentWithDoctors.FirstOrDefault(d => d.DeptNo == id);
            if (departmentToDelete != null && departmentToDelete.Doctors.Any())
            {
                _logger.LogWarning("Cannot delete department {DepartmentName} (ID: {DepartmentId}) as it has {DoctorCount} doctors assigned",
                    department.DeptName, id, departmentToDelete.Doctors.Count);
                TempData["ErrorMessage"] = "Cannot delete department as it has doctors assigned. Reassign doctors first.";
                return RedirectToPage();
            }

            try
            {
                _unitOfWork.Departments.Delete(department);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Department {DepartmentName} (ID: {DepartmentId}) deleted successfully",
                    department.DeptName, id);
                TempData["SuccessMessage"] = "Department deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
                TempData["ErrorMessage"] = "Error deleting department. Please try again.";
            }

            return RedirectToPage();
        }
    }
}