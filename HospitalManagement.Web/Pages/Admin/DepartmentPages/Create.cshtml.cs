using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Web.Pages.Admin.DepartmentPages
{
    [Authorize(Policy = "RequireAdminRole")]
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IUnitOfWork unitOfWork, ILogger<CreateModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public DepartmentCreateViewModel DepartmentModel { get; set; } = new DepartmentCreateViewModel();

        public class DepartmentCreateViewModel
        {
            [Required(ErrorMessage = "Department name is required")]
            [StringLength(30, ErrorMessage = "Department name cannot exceed 30 characters")]
            [Display(Name = "Department Name")]
            public string DeptName { get; set; } = string.Empty;

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            [Display(Name = "Description")]
            public string? Description { get; set; }
        }

        public void OnGet()
        {
            // Nothing specific needed for the initial GET request
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if a department with the same name already exists
                var existingDepartment = await _unitOfWork.Departments.GetByNameAsync(DepartmentModel.DeptName);
                if (existingDepartment != null)
                {
                    ModelState.AddModelError("DepartmentModel.DeptName", "A department with this name already exists.");
                    return Page();
                }

                // Create new department
                var department = new HospitalManagement.Domain.Models.Department
                {
                    DeptName = DepartmentModel.DeptName,
                    Description = DepartmentModel.Description
                };

                // Add to database
                await _unitOfWork.Departments.AddAsync(department);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Department {DepartmentName} created successfully", department.DeptName);
                TempData["SuccessMessage"] = "Department created successfully.";
                return RedirectToPage("/Admin/Departments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department {DepartmentName}", DepartmentModel.DeptName);
                TempData["ErrorMessage"] = "Error creating department. Please try again.";
                return Page();
            }
        }
    }
}