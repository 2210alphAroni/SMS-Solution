using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private readonly SubjectService _subjectService;
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private readonly AuthService _authService;

        public SubjectsController(SubjectService subjectService, ClassService classService,
            TeacherService teacherService, AuthService authService)
        {
            _subjectService = subjectService;
            _classService = classService;
            _teacherService = teacherService;
            _authService = authService;
        }

        public async Task<IActionResult> Index(string? classId)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var subjects = await _subjectService.GetAllAsync(tenantId, classId);
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Teachers = await _teacherService.GetAllAsync(tenantId);
            ViewBag.SelectedClass = classId;
            return View(subjects);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Teachers = await _teacherService.GetAllAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _subjectService.CreateAsync(model);
            TempData["Success"] = $"Subject '{model.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var subject = await _subjectService.GetByIdAsync(id, tenantId);
            if (subject == null) return NotFound();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Teachers = await _teacherService.GetAllAsync(tenantId);
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Subject model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _subjectService.UpdateAsync(model);
            TempData["Success"] = "Subject updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _subjectService.DeleteAsync(id, tenantId);
            TempData["Success"] = "Subject deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetByClass(string classId)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var subjects = await _subjectService.GetAllAsync(tenantId, classId);
            return Json(subjects.Select(s => new { s.Id, s.Name, s.Code, s.TotalMarks }));
        }
    }

    [Authorize]
    public class AcademicYearsController : Controller
    {
        private readonly AcademicYearService _academicYearService;
        private readonly AuthService _authService;

        public AcademicYearsController(AcademicYearService academicYearService, AuthService authService)
        {
            _academicYearService = academicYearService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var years = await _academicYearService.GetAllAsync(tenantId);
            return View(years);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicYear model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _academicYearService.CreateAsync(model);
            TempData["Success"] = $"Academic year '{model.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCurrent(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _academicYearService.SetCurrentAsync(id, tenantId);
            TempData["Success"] = "Current academic year updated.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public class SettingsController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly AuthService _authService;

        public SettingsController(TenantService tenantService, AuthService authService)
        {
            _tenantService = tenantService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            return View(tenant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(Tenant model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.Id = tenantId;
            await _tenantService.UpdateAsync(model);
            TempData["Success"] = "School settings updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly AuthService _authService;
        private readonly MongoDbContext _db;

        public SubscriptionController(TenantService tenantService, AuthService authService, MongoDbContext db)
        {
            _tenantService = tenantService;
            _authService = authService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            var plans = await _db.SubscriptionPlans.Find(_ => true).ToListAsync();
            ViewBag.Tenant = tenant;
            return View(plans);
        }
    }

    // SuperAdmin controller for platform management
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly MongoDbContext _db;

        public SuperAdminController(TenantService tenantService, MongoDbContext db)
        {
            _tenantService = tenantService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _tenantService.GetAllAsync();
            var totalStudents = await _db.Students.CountDocumentsAsync(_ => true);
            var totalTeachers = await _db.Teachers.CountDocumentsAsync(_ => true);
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalTeachers = totalTeachers;
            return View(tenants);
        }

        public async Task<IActionResult> Tenants()
        {
            var tenants = await _tenantService.GetAllAsync();
            return View(tenants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTenant(string id, bool activate)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant != null)
            {
                tenant.IsActive = activate;
                await _tenantService.UpdateAsync(tenant);
                TempData["Success"] = $"School {(activate ? "activated" : "deactivated")} successfully.";
            }
            return RedirectToAction(nameof(Tenants));
        }
    }
}
