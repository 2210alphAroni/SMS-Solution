using MongoDB.Driver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    // ==================== TEACHER SERVICE ====================
    public class TeacherService
    {
        private readonly MongoDbContext _context;
        public TeacherService(MongoDbContext context) => _context = context;

        public async Task<List<Teacher>> GetAllAsync(string tenantId, string? status = null)
        {
            var filter = Builders<Teacher>.Filter.Eq(t => t.TenantId, tenantId);
            if (!string.IsNullOrEmpty(status)) filter &= Builders<Teacher>.Filter.Eq(t => t.Status, status);
            return await _context.Teachers.Find(filter).SortBy(t => t.FirstName).ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Teachers.Find(t => t.Id == id && t.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Teacher> CreateAsync(Teacher teacher)
        {
            var count = await _context.Teachers.CountDocumentsAsync(t => t.TenantId == teacher.TenantId);
            teacher.EmployeeId = $"EMP{DateTime.UtcNow.Year}{(count + 1):D4}";
            teacher.CreatedAt = DateTime.UtcNow;
            teacher.UpdatedAt = DateTime.UtcNow;
            await _context.Teachers.InsertOneAsync(teacher);
            return teacher;
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            teacher.UpdatedAt = DateTime.UtcNow;
            await _context.Teachers.ReplaceOneAsync(t => t.Id == teacher.Id && t.TenantId == teacher.TenantId, teacher);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Teachers.DeleteOneAsync(t => t.Id == id && t.TenantId == tenantId);
        }

        public async Task<long> CountAsync(string tenantId)
        {
            return await _context.Teachers.CountDocumentsAsync(t => t.TenantId == tenantId);
        }
    }

    // ==================== CLASS SERVICE ====================
    public class ClassService
    {
        private readonly MongoDbContext _context;
        public ClassService(MongoDbContext context) => _context = context;

        public async Task<List<Class>> GetAllAsync(string tenantId, string? academicYearId = null)
        {
            var filter = Builders<Class>.Filter.Eq(c => c.TenantId, tenantId);
            if (!string.IsNullOrEmpty(academicYearId)) filter &= Builders<Class>.Filter.Eq(c => c.AcademicYearId, academicYearId);
            return await _context.Classes.Find(filter).SortBy(c => c.Order).ToListAsync();
        }

        public async Task<Class?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Classes.Find(c => c.Id == id && c.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Class> CreateAsync(Class cls)
        {
            cls.CreatedAt = DateTime.UtcNow;
            await _context.Classes.InsertOneAsync(cls);
            return cls;
        }

        public async Task UpdateAsync(Class cls)
        {
            await _context.Classes.ReplaceOneAsync(c => c.Id == cls.Id && c.TenantId == cls.TenantId, cls);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Classes.DeleteOneAsync(c => c.Id == id && c.TenantId == tenantId);
        }
    }

    // ==================== SUBJECT SERVICE ====================
    public class SubjectService
    {
        private readonly MongoDbContext _context;
        public SubjectService(MongoDbContext context) => _context = context;

        public async Task<List<Subject>> GetAllAsync(string tenantId, string? classId = null)
        {
            var filter = Builders<Subject>.Filter.Eq(s => s.TenantId, tenantId);
            if (!string.IsNullOrEmpty(classId)) filter &= Builders<Subject>.Filter.Eq(s => s.ClassId, classId);
            return await _context.Subjects.Find(filter).SortBy(s => s.Name).ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Subjects.Find(s => s.Id == id && s.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            subject.CreatedAt = DateTime.UtcNow;
            await _context.Subjects.InsertOneAsync(subject);
            return subject;
        }

        public async Task UpdateAsync(Subject subject)
        {
            await _context.Subjects.ReplaceOneAsync(s => s.Id == subject.Id && s.TenantId == subject.TenantId, subject);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Subjects.DeleteOneAsync(s => s.Id == id && s.TenantId == tenantId);
        }
    }

    // ==================== ATTENDANCE SERVICE ====================
    public class AttendanceService
    {
        private readonly MongoDbContext _context;
        public AttendanceService(MongoDbContext context) => _context = context;

        public async Task<Attendance?> GetByDateAsync(string tenantId, string classId, string section, DateTime date, string? subjectId = null)
        {
            var filter = Builders<Attendance>.Filter.And(
                Builders<Attendance>.Filter.Eq(a => a.TenantId, tenantId),
                Builders<Attendance>.Filter.Eq(a => a.ClassId, classId),
                Builders<Attendance>.Filter.Eq(a => a.Section, section),
                Builders<Attendance>.Filter.Eq(a => a.Date, date.Date)
            );
            if (!string.IsNullOrEmpty(subjectId)) filter &= Builders<Attendance>.Filter.Eq(a => a.SubjectId, subjectId);
            return await _context.Attendances.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Attendance> SaveAttendanceAsync(Attendance attendance)
        {
            attendance.Date = attendance.Date.Date;
            var existing = await GetByDateAsync(attendance.TenantId, attendance.ClassId, attendance.Section, attendance.Date, attendance.SubjectId);
            if (existing != null)
            {
                attendance.Id = existing.Id;
                await _context.Attendances.ReplaceOneAsync(a => a.Id == existing.Id, attendance);
            }
            else
            {
                attendance.CreatedAt = DateTime.UtcNow;
                await _context.Attendances.InsertOneAsync(attendance);
            }
            return attendance;
        }

        public async Task<Dictionary<string, int>> GetStudentAttendanceSummaryAsync(string tenantId, string studentId, string academicYearId)
        {
            var records = await _context.Attendances
                .Find(a => a.TenantId == tenantId && a.AcademicYearId == academicYearId)
                .ToListAsync();

            int present = 0, absent = 0, late = 0, leave = 0;
            foreach (var rec in records)
            {
                var studentRecord = rec.Records.FirstOrDefault(r => r.StudentId == studentId);
                if (studentRecord != null)
                {
                    switch (studentRecord.Status)
                    {
                        case "Present": present++; break;
                        case "Absent": absent++; break;
                        case "Late": late++; break;
                        case "Leave": leave++; break;
                    }
                }
            }
            return new Dictionary<string, int>
            {
                { "Present", present }, { "Absent", absent },
                { "Late", late }, { "Leave", leave },
                { "Total", present + absent + late + leave }
            };
        }

        public async Task<List<Attendance>> GetClassAttendanceAsync(string tenantId, string classId, string section, DateTime from, DateTime to)
        {
            return await _context.Attendances.Find(a =>
                a.TenantId == tenantId && a.ClassId == classId &&
                a.Section == section && a.Date >= from && a.Date <= to)
                .SortBy(a => a.Date)
                .ToListAsync();
        }
    }

    // ==================== FEE SERVICE ====================
    public class FeeService
    {
        private readonly MongoDbContext _context;
        public FeeService(MongoDbContext context) => _context = context;

        public async Task<List<FeeStructure>> GetStructuresAsync(string tenantId, string? classId = null)
        {
            var filter = Builders<FeeStructure>.Filter.Eq(f => f.TenantId, tenantId);
            if (!string.IsNullOrEmpty(classId)) filter &= Builders<FeeStructure>.Filter.Eq(f => f.ClassId, classId);
            return await _context.FeeStructures.Find(filter).ToListAsync();
        }

        public async Task<FeeStructure> CreateStructureAsync(FeeStructure structure)
        {
            structure.CreatedAt = DateTime.UtcNow;
            await _context.FeeStructures.InsertOneAsync(structure);
            return structure;
        }

        public async Task<List<FeePayment>> GetPaymentsAsync(string tenantId, string? studentId = null, string? month = null, string? status = null)
        {
            var filter = Builders<FeePayment>.Filter.Eq(f => f.TenantId, tenantId);
            if (!string.IsNullOrEmpty(studentId)) filter &= Builders<FeePayment>.Filter.Eq(f => f.StudentId, studentId);
            if (!string.IsNullOrEmpty(month)) filter &= Builders<FeePayment>.Filter.Eq(f => f.Month, month);
            if (!string.IsNullOrEmpty(status)) filter &= Builders<FeePayment>.Filter.Eq(f => f.Status, status);
            return await _context.FeePayments.Find(filter).SortByDescending(f => f.PaymentDate).ToListAsync();
        }

        public async Task<FeePayment?> GetPaymentByIdAsync(string id, string tenantId)
        {
            return await _context.FeePayments.Find(f => f.Id == id && f.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<FeePayment> CreatePaymentAsync(FeePayment payment)
        {
            var count = await _context.FeePayments.CountDocumentsAsync(f => f.TenantId == payment.TenantId);
            payment.ReceiptNo = $"RCP{DateTime.UtcNow.Year}{(count + 1):D6}";
            payment.CreatedAt = DateTime.UtcNow;
            await _context.FeePayments.InsertOneAsync(payment);
            return payment;
        }

        public async Task<decimal> GetTotalCollectedAsync(string tenantId, DateTime from, DateTime to)
        {
            var payments = await _context.FeePayments
                .Find(f => f.TenantId == tenantId && f.PaymentDate >= from && f.PaymentDate <= to)
                .ToListAsync();
            return payments.Sum(p => p.PaidAmount);
        }
    }

    // ==================== EXAM SERVICE ====================
    public class ExamService
    {
        private readonly MongoDbContext _context;
        public ExamService(MongoDbContext context) => _context = context;

        public async Task<List<Exam>> GetAllAsync(string tenantId, string? academicYearId = null, string? classId = null)
        {
            var filter = Builders<Exam>.Filter.Eq(e => e.TenantId, tenantId);
            if (!string.IsNullOrEmpty(academicYearId)) filter &= Builders<Exam>.Filter.Eq(e => e.AcademicYearId, academicYearId);
            if (!string.IsNullOrEmpty(classId)) filter &= Builders<Exam>.Filter.Eq(e => e.ClassId, classId);
            return await _context.Exams.Find(filter).SortByDescending(e => e.StartDate).ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Exams.Find(e => e.Id == id && e.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Exam> CreateAsync(Exam exam)
        {
            exam.CreatedAt = DateTime.UtcNow;
            await _context.Exams.InsertOneAsync(exam);
            return exam;
        }

        public async Task UpdateAsync(Exam exam)
        {
            await _context.Exams.ReplaceOneAsync(e => e.Id == exam.Id && e.TenantId == exam.TenantId, exam);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Exams.DeleteOneAsync(e => e.Id == id && e.TenantId == tenantId);
        }

        public async Task<ExamResult> SaveResultAsync(ExamResult result)
        {
            result.Percentage = result.TotalMarks > 0 ? Math.Round((result.ObtainedMarks / result.TotalMarks) * 100, 2) : 0;
            result.Grade = CalculateGrade(result.Percentage);
            result.Status = result.ObtainedMarks >= (result.TotalMarks * 0.33m) ? "Pass" : "Fail";
            result.CreatedAt = DateTime.UtcNow;
            var existing = await _context.Results.Find(r => r.TenantId == result.TenantId && r.ExamId == result.ExamId && r.StudentId == result.StudentId).FirstOrDefaultAsync();
            if (existing != null)
            {
                result.Id = existing.Id;
                await _context.Results.ReplaceOneAsync(r => r.Id == existing.Id, result);
            }
            else
            {
                await _context.Results.InsertOneAsync(result);
            }
            return result;
        }

        public async Task<List<ExamResult>> GetResultsByExamAsync(string tenantId, string examId)
        {
            return await _context.Results.Find(r => r.TenantId == tenantId && r.ExamId == examId).ToListAsync();
        }

        public async Task<List<ExamResult>> GetResultsByStudentAsync(string tenantId, string studentId)
        {
            return await _context.Results.Find(r => r.TenantId == tenantId && r.StudentId == studentId).SortByDescending(r => r.CreatedAt).ToListAsync();
        }

        private string CalculateGrade(decimal percentage) => percentage switch
        {
            >= 80 => "A+",
            >= 70 => "A",
            >= 60 => "B",
            >= 50 => "C",
            >= 40 => "D",
            _ => "F"
        };
    }

    // ==================== NOTICE SERVICE ====================
    public class NoticeService
    {
        private readonly MongoDbContext _context;
        public NoticeService(MongoDbContext context) => _context = context;

        public async Task<List<Notice>> GetAllAsync(string tenantId, bool activeOnly = true)
        {
            var filter = Builders<Notice>.Filter.Eq(n => n.TenantId, tenantId);
            if (activeOnly)
            {
                filter &= Builders<Notice>.Filter.Eq(n => n.IsActive, true);
                filter &= Builders<Notice>.Filter.Gte(n => n.ExpiryDate, DateTime.UtcNow);
            }
            return await _context.Notices.Find(filter).SortByDescending(n => n.IsPinned).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<Notice?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Notices.Find(n => n.Id == id && n.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Notice> CreateAsync(Notice notice)
        {
            notice.CreatedAt = DateTime.UtcNow;
            await _context.Notices.InsertOneAsync(notice);
            return notice;
        }

        public async Task UpdateAsync(Notice notice)
        {
            await _context.Notices.ReplaceOneAsync(n => n.Id == notice.Id && n.TenantId == notice.TenantId, notice);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Notices.DeleteOneAsync(n => n.Id == id && n.TenantId == tenantId);
        }
    }

    // ==================== ACADEMIC YEAR SERVICE ====================
    public class AcademicYearService
    {
        private readonly MongoDbContext _context;
        public AcademicYearService(MongoDbContext context) => _context = context;

        public async Task<List<AcademicYear>> GetAllAsync(string tenantId)
        {
            return await _context.AcademicYears.Find(a => a.TenantId == tenantId).SortByDescending(a => a.StartDate).ToListAsync();
        }

        public async Task<AcademicYear?> GetCurrentAsync(string tenantId)
        {
            return await _context.AcademicYears.Find(a => a.TenantId == tenantId && a.IsCurrentYear).FirstOrDefaultAsync();
        }

        public async Task<AcademicYear?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.AcademicYears.Find(a => a.Id == id && a.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<AcademicYear> CreateAsync(AcademicYear year)
        {
            if (year.IsCurrentYear)
            {
                var update = Builders<AcademicYear>.Update.Set(a => a.IsCurrentYear, false);
                await _context.AcademicYears.UpdateManyAsync(a => a.TenantId == year.TenantId, update);
            }
            year.CreatedAt = DateTime.UtcNow;
            await _context.AcademicYears.InsertOneAsync(year);
            return year;
        }

        public async Task SetCurrentAsync(string id, string tenantId)
        {
            await _context.AcademicYears.UpdateManyAsync(a => a.TenantId == tenantId,
                Builders<AcademicYear>.Update.Set(a => a.IsCurrentYear, false));
            await _context.AcademicYears.UpdateOneAsync(a => a.Id == id && a.TenantId == tenantId,
                Builders<AcademicYear>.Update.Set(a => a.IsCurrentYear, true));
        }
    }
}
