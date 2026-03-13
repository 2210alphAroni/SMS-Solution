using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModels;

namespace SchoolManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly TenantService _tenantService;

        public AccountController(AuthService authService, TenantService tenantService)
        {
            _authService = authService;
            _tenantService = tenantService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var tenant = await _tenantService.GetBySubdomainAsync(model.Subdomain);
            if (tenant == null || !tenant.IsActive)
            {
                ModelState.AddModelError("", "School not found or inactive. Please check your school code.");
                return View(model);
            }

            var user = await _authService.AuthenticateAsync(tenant.Id!, model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            await _authService.SignInAsync(user, tenant, model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterTenantViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Subdomain = model.Subdomain.ToLower().Trim();

            if (await _tenantService.SubdomainExistsAsync(model.Subdomain))
            {
                ModelState.AddModelError("Subdomain", "This school code is already taken. Please choose another.");
                return View(model);
            }

            if (await _tenantService.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var tenant = new Tenant
            {
                SchoolName = model.SchoolName,
                Subdomain = model.Subdomain,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                PrincipalName = model.PrincipalName,
                SubscriptionPlan = "Free",
                IsActive = true,
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = DateTime.UtcNow.AddDays(30)
            };

            var createdTenant = await _tenantService.CreateAsync(tenant);
            var adminUser = await _authService.CreateUserAsync(
                createdTenant.Id!,
                model.Email,
                model.Password,
                model.PrincipalName.Split(' ')[0],
                model.PrincipalName.Split(' ').Length > 1 ? model.PrincipalName.Split(' ')[1] : "",
                "Admin"
            );

            await _authService.SignInAsync(adminUser, createdTenant);
            TempData["Success"] = $"Welcome to EduManage Pro! Your school '{model.SchoolName}' has been created successfully.";
            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _authService.GetCurrentUserId();
            var user = await _authService.GetUserByIdAsync(userId);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Profile");
            var userId = _authService.GetCurrentUserId();
            var success = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
            if (!success) TempData["Error"] = "Current password is incorrect.";
            else TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult AccessDenied() => View();

        public IActionResult ForgotPassword() => View();
    }
}
