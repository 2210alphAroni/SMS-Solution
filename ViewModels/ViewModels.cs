using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "School code is required")]
        [Display(Name = "School Code")]
        public string Subdomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterTenantViewModel
    {
        [Required(ErrorMessage = "School name is required")]
        [Display(Name = "School Name")]
        public string SchoolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "School code is required")]
        [Display(Name = "School Code (URL slug)")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Only lowercase letters, numbers and hyphens allowed")]
        [MinLength(3), MaxLength(30)]
        public string Subdomain { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Principal's Name")]
        public string PrincipalName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = "Bangladesh";

        [Required, MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool AgreeToTerms { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
