using System.ComponentModel.DataAnnotations;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HospitalManagement.Web.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IUnitOfWork unitOfWork, ILogger<RegisterModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(RegisterInput.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already registered.");
                    return Page();
                }

                // Create new user (patient)
                var user = new User
                {
                    Email = RegisterInput.Email,
                    Password = RegisterInput.Password,
                    Type = 2 // Patient type
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Create patient profile
                var patient = new Patient
                {
                    Id = user.Id,
                    Name = RegisterInput.Name,
                    Phone = RegisterInput.Phone,
                    Address = RegisterInput.Address,
                    BirthDate = RegisterInput.BirthDate,
                    Gender = RegisterInput.Gender
                };

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User {Email} created a new account.", user.Email);

                // Sign in the new user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Type.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToPage("/Patients/Dashboard", new { id = user.Id });
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }

    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = null!;

        [StringLength(11)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [StringLength(40)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-20);

        [Required]
        [StringLength(1)]
        [Display(Name = "Gender (M/F)")]
        public string Gender { get; set; } = null!;
    }
}