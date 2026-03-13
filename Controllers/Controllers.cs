using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.Models;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly AuthService _authService;

        public DashboardController(TenantService tenantService, AuthService authService)
        {
            _tenantService = tenantService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var stats = await _tenantService.GetDashboardStatsAsync(tenantId);
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            ViewBag.Tenant = tenant;
            return View(stats);
        }
    }

    [Authorize]
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;
        private readonly AcademicYearService _academicYearService;

        public StudentsController(StudentService studentService, ClassService classService, AuthService authService, AcademicYearService academicYearService)
        {
            _studentService = studentService;
            _classService = classService;
            _authService = authService;
            _academicYearService = academicYearService;
        }

        public async Task<IActionResult> Index(string? classId, string? section, string? status, string? search)
        {
            var tenantId = _authService.GetCurrentTenantId();
            List<Student> students;
            if (!string.IsNullOrEmpty(search))
                students = await _studentService.SearchAsync(tenantId, search);
            else
                students = await _studentService.GetAllAsync(tenantId, classId, section, status ?? "Active");

            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.SelectedClass = classId;
            ViewBag.SelectedSection = section;
            ViewBag.SelectedStatus = status ?? "Active";
            ViewBag.Search = search;
            return View(students);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.AcademicYears = await _academicYearService.GetAllAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            var student = await _studentService.CreateAsync(model);
            TempData["Success"] = $"Student '{student.FullName}' added successfully. Admission No: {student.AdmissionNo}";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var student = await _studentService.GetByIdAsync(id, tenantId);
            if (student == null) return NotFound();
            return View(student);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var student = await _studentService.GetByIdAsync(id, tenantId);
            if (student == null) return NotFound();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _studentService.UpdateAsync(model);
            TempData["Success"] = "Student updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _studentService.DeleteAsync(id, tenantId);
            TempData["Success"] = "Student deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var students = await _studentService.SearchAsync(tenantId, q);
            return Json(students.Select(s => new { s.Id, name = s.FullName, s.AdmissionNo, s.ClassId }));
        }
    }

    [Authorize]
    public class TeachersController : Controller
    {
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly SubjectService _subjectService;
        private readonly AuthService _authService;

        public TeachersController(TeacherService teacherService, ClassService classService, SubjectService subjectService, AuthService authService)
        {
            _teacherService = teacherService;
            _classService = classService;
            _subjectService = subjectService;
            _authService = authService;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var teachers = await _teacherService.GetAllAsync(tenantId, status);
            ViewBag.SelectedStatus = status;
            return View(teachers);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Subjects = await _subjectService.GetAllAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            var teacher = await _teacherService.CreateAsync(model);
            TempData["Success"] = $"Teacher '{teacher.FullName}' added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var teacher = await _teacherService.GetByIdAsync(id, tenantId);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var teacher = await _teacherService.GetByIdAsync(id, tenantId);
            if (teacher == null) return NotFound();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Subjects = await _subjectService.GetAllAsync(tenantId);
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _teacherService.UpdateAsync(model);
            TempData["Success"] = "Teacher updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _teacherService.DeleteAsync(id, tenantId);
            TempData["Success"] = "Teacher deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public class ClassesController : Controller
    {
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly AuthService _authService;
        private readonly AcademicYearService _academicYearService;

        public ClassesController(ClassService classService, TeacherService teacherService, SubjectService subjectService, AuthService authService, AcademicYearService academicYearService)
        {
            _classService = classService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _authService = authService;
            _academicYearService = academicYearService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var currentYear = await _academicYearService.GetCurrentAsync(tenantId);
            var classes = await _classService.GetAllAsync(tenantId, currentYear?.Id);
            ViewBag.CurrentYear = currentYear;
            return View(classes);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Teachers = await _teacherService.GetAllAsync(tenantId);
            ViewBag.AcademicYears = await _academicYearService.GetAllAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _classService.CreateAsync(model);
            TempData["Success"] = $"Class '{model.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var cls = await _classService.GetByIdAsync(id, tenantId);
            if (cls == null) return NotFound();
            ViewBag.Teachers = await _teacherService.GetAllAsync(tenantId);
            return View(cls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Class model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _classService.UpdateAsync(model);
            TempData["Success"] = "Class updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _classService.DeleteAsync(id, tenantId);
            TempData["Success"] = "Class deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly AttendanceService _attendanceService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;

        public AttendanceController(AttendanceService attendanceService, StudentService studentService, ClassService classService, AuthService authService)
        {
            _attendanceService = attendanceService;
            _studentService = studentService;
            _classService = classService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TakeAttendance(string classId, string section, string date)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var parsedDate = DateTime.Parse(date);
            var students = await _studentService.GetAllAsync(tenantId, classId, section, "Active");
            var existing = await _attendanceService.GetByDateAsync(tenantId, classId, section, parsedDate);
            var cls = await _classService.GetByIdAsync(classId, tenantId);

            ViewBag.Class = cls;
            ViewBag.Section = section;
            ViewBag.Date = parsedDate;
            ViewBag.ExistingAttendance = existing;
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequest request)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var teacherId = _authService.GetCurrentUserId();
            var attendance = new Attendance
            {
                TenantId = tenantId,
                ClassId = request.ClassId,
                Section = request.Section,
                TeacherId = teacherId,
                Date = DateTime.Parse(request.Date),
                Records = request.Records.Select(r => new AttendanceRecord
                {
                    StudentId = r.StudentId,
                    Status = r.Status,
                    Remark = r.Remark ?? ""
                }).ToList()
            };
            await _attendanceService.SaveAttendanceAsync(attendance);
            return Json(new { success = true, message = "Attendance saved successfully." });
        }

        public async Task<IActionResult> Report(string classId, string section, string from, string to)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var fromDate = DateTime.Parse(from);
            var toDate = DateTime.Parse(to);
            var records = await _attendanceService.GetClassAttendanceAsync(tenantId, classId, section, fromDate, toDate);
            var students = await _studentService.GetAllAsync(tenantId, classId, section);
            ViewBag.Students = students;
            ViewBag.From = fromDate;
            ViewBag.To = toDate;
            return View(records);
        }
    }

    public class SaveAttendanceRequest
    {
        public string ClassId { get; set; } = "";
        public string Section { get; set; } = "";
        public string Date { get; set; } = "";
        public List<AttendanceRecordRequest> Records { get; set; } = new();
    }

    public class AttendanceRecordRequest
    {
        public string StudentId { get; set; } = "";
        public string Status { get; set; } = "Present";
        public string? Remark { get; set; }
    }

    [Authorize]
    public class ExamsController : Controller
    {
        private readonly ExamService _examService;
        private readonly ClassService _classService;
        private readonly SubjectService _subjectService;
        private readonly StudentService _studentService;
        private readonly AuthService _authService;
        private readonly AcademicYearService _academicYearService;

        public ExamsController(ExamService examService, ClassService classService, SubjectService subjectService, StudentService studentService, AuthService authService, AcademicYearService academicYearService)
        {
            _examService = examService;
            _classService = classService;
            _subjectService = subjectService;
            _studentService = studentService;
            _authService = authService;
            _academicYearService = academicYearService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var currentYear = await _academicYearService.GetCurrentAsync(tenantId);
            var exams = await _examService.GetAllAsync(tenantId, currentYear?.Id);
            return View(exams);
        }

        public async Task<IActionResult> Create()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Subjects = await _subjectService.GetAllAsync(tenantId);
            ViewBag.AcademicYears = await _academicYearService.GetAllAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _examService.CreateAsync(model);
            TempData["Success"] = $"Exam '{model.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EnterResults(string examId)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var exam = await _examService.GetByIdAsync(examId, tenantId);
            if (exam == null) return NotFound();
            var students = await _studentService.GetAllAsync(tenantId, exam.ClassId, exam.Section, "Active");
            var subjects = await _subjectService.GetAllAsync(tenantId, exam.ClassId);
            var existingResults = await _examService.GetResultsByExamAsync(tenantId, examId);
            ViewBag.Exam = exam;
            ViewBag.Subjects = subjects;
            ViewBag.ExistingResults = existingResults;
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResults([FromBody] SaveResultsRequest request)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var exam = await _examService.GetByIdAsync(request.ExamId, tenantId);
            if (exam == null) return Json(new { success = false, message = "Exam not found" });

            foreach (var sr in request.StudentResults)
            {
                var result = new ExamResult
                {
                    TenantId = tenantId,
                    ExamId = request.ExamId,
                    StudentId = sr.StudentId,
                    ClassId = exam.ClassId,
                    Section = exam.Section,
                    AcademicYearId = exam.AcademicYearId,
                    SubjectResults = sr.Subjects.Select(s => new SubjectResult
                    {
                        SubjectId = s.SubjectId,
                        TotalMarks = s.TotalMarks,
                        ObtainedMarks = s.ObtainedMarks,
                        IsPassed = s.ObtainedMarks >= (s.TotalMarks * 0.33m)
                    }).ToList(),
                    TotalMarks = sr.Subjects.Sum(s => s.TotalMarks),
                    ObtainedMarks = sr.Subjects.Sum(s => s.ObtainedMarks)
                };
                await _examService.SaveResultAsync(result);
            }
            return Json(new { success = true, message = "Results saved successfully." });
        }
    }

    public class SaveResultsRequest
    {
        public string ExamId { get; set; } = "";
        public List<StudentResultInput> StudentResults { get; set; } = new();
    }

    public class StudentResultInput
    {
        public string StudentId { get; set; } = "";
        public List<SubjectResultInput> Subjects { get; set; } = new();
    }

    public class SubjectResultInput
    {
        public string SubjectId { get; set; } = "";
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
    }

    [Authorize]
    public class FeesController : Controller
    {
        private readonly FeeService _feeService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;

        public FeesController(FeeService feeService, StudentService studentService, ClassService classService, AuthService authService)
        {
            _feeService = feeService;
            _studentService = studentService;
            _classService = classService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var payments = await _feeService.GetPaymentsAsync(tenantId);
            var today = DateTime.UtcNow;
            ViewBag.TotalCollected = await _feeService.GetTotalCollectedAsync(tenantId, today.AddDays(-today.Day + 1), today);
            return View(payments);
        }

        public async Task<IActionResult> Structures()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var structures = await _feeService.GetStructuresAsync(tenantId);
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            return View(structures);
        }

        public async Task<IActionResult> Collect()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            ViewBag.Structures = await _feeService.GetStructuresAsync(tenantId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Collect(FeePayment model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            model.CollectedBy = _authService.GetCurrentUserId();
            model.DueAmount = model.TotalAmount - model.PaidAmount - model.DiscountAmount + model.FineAmount;
            model.Status = model.DueAmount <= 0 ? "Paid" : model.PaidAmount > 0 ? "Partial" : "Unpaid";
            var payment = await _feeService.CreatePaymentAsync(model);
            TempData["Success"] = $"Fee collected. Receipt No: {payment.ReceiptNo}";
            return RedirectToAction("Receipt", new { id = payment.Id });
        }

        public async Task<IActionResult> Receipt(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var payment = await _feeService.GetPaymentByIdAsync(id, tenantId);
            if (payment == null) return NotFound();
            var student = await _studentService.GetByIdAsync(payment.StudentId, tenantId);
            ViewBag.Student = student;
            return View(payment);
        }
    }

    [Authorize]
    public class NoticesController : Controller
    {
        private readonly NoticeService _noticeService;
        private readonly AuthService _authService;

        public NoticesController(NoticeService noticeService, AuthService authService)
        {
            _noticeService = noticeService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var notices = await _noticeService.GetAllAsync(tenantId);
            return View(notices);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notice model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            model.CreatedBy = _authService.GetCurrentUserId();
            await _noticeService.CreateAsync(model);
            TempData["Success"] = "Notice published successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var notice = await _noticeService.GetByIdAsync(id, tenantId);
            if (notice == null) return NotFound();
            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Notice model)
        {
            var tenantId = _authService.GetCurrentTenantId();
            model.TenantId = tenantId;
            await _noticeService.UpdateAsync(model);
            TempData["Success"] = "Notice updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = _authService.GetCurrentTenantId();
            await _noticeService.DeleteAsync(id, tenantId);
            TempData["Success"] = "Notice deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AuthService _authService;
        private readonly StudentService _studentService;
        private readonly AttendanceService _attendanceService;
        private readonly FeeService _feeService;
        private readonly ClassService _classService;

        public ReportsController(AuthService authService, StudentService studentService, AttendanceService attendanceService, FeeService feeService, ClassService classService)
        {
            _authService = authService;
            _studentService = studentService;
            _attendanceService = attendanceService;
            _feeService = feeService;
            _classService = classService;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _authService.GetCurrentTenantId();
            ViewBag.Classes = await _classService.GetAllAsync(tenantId);
            return View();
        }

        public async Task<IActionResult> Students()
        {
            var tenantId = _authService.GetCurrentTenantId();
            var students = await _studentService.GetAllAsync(tenantId);
            var classes = await _classService.GetAllAsync(tenantId);
            var classDict = classes.ToDictionary(c => c.Id!, c => c.Name);
            ViewBag.ClassDict = classDict;
            return View(students);
        }

        public async Task<IActionResult> FeeCollection(string from, string to)
        {
            var tenantId = _authService.GetCurrentTenantId();
            var fromDate = string.IsNullOrEmpty(from) ? DateTime.UtcNow.AddMonths(-1) : DateTime.Parse(from);
            var toDate = string.IsNullOrEmpty(to) ? DateTime.UtcNow : DateTime.Parse(to);
            var payments = await _feeService.GetPaymentsAsync(tenantId);
            var filtered = payments.Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate).ToList();
            ViewBag.From = fromDate;
            ViewBag.To = toDate;
            ViewBag.Total = filtered.Sum(p => p.PaidAmount);
            return View(filtered);
        }
    }
}
