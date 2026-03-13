using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SchoolManagementSystem.Models
{
    // ==================== TENANT / SCHOOL ====================
    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty; // e.g., "greenvalley" for greenvalley.edumanagepro.com
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string PrincipalName { get; set; } = string.Empty;
        public string EstablishedYear { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = "Free"; // Free, Basic, Professional, Enterprise
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        public DateTime SubscriptionEndDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string StripeCustomerId { get; set; } = string.Empty;
        public string StripeSubscriptionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public SchoolSettings Settings { get; set; } = new SchoolSettings();
    }

    public class SchoolSettings
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string Currency { get; set; } = "BDT";
        public string TimeZone { get; set; } = "Asia/Dhaka";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public bool EnableOnlineFeePayment { get; set; } = false;
        public bool EnableSmsNotification { get; set; } = false;
        public bool EnableEmailNotification { get; set; } = true;
        public List<string> WorkingDays { get; set; } = new() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
        public string SchoolStartTime { get; set; } = "08:00";
        public string SchoolEndTime { get; set; } = "14:00";
        public string ThemeColor { get; set; } = "#4f46e5";
    }

    // ==================== USER / AUTH ====================
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "Teacher"; // SuperAdmin, Admin, Teacher, Student, Parent
        public string Phone { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string EmailVerificationToken { get; set; } = string.Empty;
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? PasswordResetExpiry { get; set; }
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }

    // ==================== ACADEMIC YEAR ====================
    public class AcademicYear
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentYear { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== CLASS / SECTION ====================
    public class Class
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // e.g., "Class 1", "Grade 10"
        public int Order { get; set; } = 0;
        public List<Section> Sections { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Section
    {
        public string Name { get; set; } = string.Empty; // A, B, C
        public string ClassTeacherId { get; set; } = string.Empty;
        public int Capacity { get; set; } = 40;
    }

    // ==================== SUBJECT ====================
    public class Subject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public int TotalMarks { get; set; } = 100;
        public int PassMarks { get; set; } = 33;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== STUDENT ====================
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string AdmissionNo { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Bangladeshi";
        public string ProfilePicture { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public StudentAddress Address { get; set; } = new();
        public string AdmissionDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string Status { get; set; } = "Active"; // Active, Inactive, Graduated, Transferred
        public string PreviousSchool { get; set; } = string.Empty;
        public List<Document> Documents { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }

    public class StudentAddress
    {
        public string PresentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Bangladesh";
    }

    public class Document
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== TEACHER ====================
    public class Teacher
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Bangladeshi";
        public string ProfilePicture { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
        public decimal Salary { get; set; } = 0;
        public string Address { get; set; } = string.Empty;
        public List<string> SubjectIds { get; set; } = new();
        public List<string> ClassIds { get; set; } = new();
        public string Status { get; set; } = "Active";
        public string BankAccount { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }

    // ==================== PARENT ====================
    public class Parent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string Relation { get; set; } = "Father"; // Father, Mother, Guardian
        public string Address { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public List<string> StudentIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }

    // ==================== ATTENDANCE ====================
    public class Attendance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public string AcademicYearId { get; set; } = string.Empty;
        public List<AttendanceRecord> Records { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AttendanceRecord
    {
        public string StudentId { get; set; } = string.Empty;
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Leave
        public string Remark { get; set; } = string.Empty;
    }

    // ==================== EXAM ====================
    public class Exam
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty; // Unit Test, Mid Term, Final
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ExamSchedule> Schedule { get; set; } = new();
        public bool IsResultPublished { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ExamSchedule
    {
        public string SubjectId { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int TotalMarks { get; set; } = 100;
        public string Room { get; set; } = string.Empty;
    }

    // ==================== RESULT ====================
    public class ExamResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public List<SubjectResult> SubjectResults { get; set; } = new();
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string GradePoint { get; set; } = string.Empty;
        public int Rank { get; set; }
        public string Status { get; set; } = "Pass"; // Pass, Fail
        public string Remarks { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SubjectResult
    {
        public string SubjectId { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal? TheoryMarks { get; set; }
        public decimal? PracticalMarks { get; set; }
        public string Grade { get; set; } = string.Empty;
        public bool IsPassed { get; set; } = true;
    }

    // ==================== FEE ====================
    public class FeeStructure
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public List<FeeComponent> Components { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string DueDate { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // এই নিচের লাইনটি নতুন যোগ করুন (এরর দূর করার জন্য)
        public string AcademicYear { get; set; } = string.Empty; 

    }

    public class FeeComponent
    {
        public string Name { get; set; } = string.Empty; // Tuition, Exam, Library, etc.
        public decimal Amount { get; set; }
        public bool IsOptional { get; set; } = false;
    }

    public class FeePayment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string FeeStructureId { get; set; } = string.Empty;
        public string ReceiptNo { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FineAmount { get; set; }
        public decimal DueAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Bank, Online, Cheque
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = "Paid"; // Paid, Partial, Unpaid, Overdue
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string CollectedBy { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== NOTICE ====================
    public class Notice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "General"; // General, Exam, Event, Holiday, Emergency
        public List<string> TargetRoles { get; set; } = new() { "All" };
        public List<string> TargetClassIds { get; set; } = new();
        public bool IsPinned { get; set; } = false;
        public string AttachmentUrl { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== TIMETABLE ====================
    public class Timetable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public List<DaySchedule> Schedule { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DaySchedule
    {
        public string Day { get; set; } = string.Empty; // Sunday, Monday, etc.
        public List<Period> Periods { get; set; } = new();
    }

    public class Period
    {
        public int PeriodNo { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public bool IsBreak { get; set; } = false;
    }

    // ==================== LEAVE ====================
    public class Leave
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string ApplicantId { get; set; } = string.Empty;
        public string ApplicantType { get; set; } = string.Empty; // Teacher, Student
        public string LeaveType { get; set; } = string.Empty; // Sick, Casual, Annual, Emergency
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string ReviewedBy { get; set; } = string.Empty;
        public string ReviewNote { get; set; } = string.Empty;
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== LIBRARY ====================
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public decimal Price { get; set; }
        public string Shelf { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class BookIssue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public string MemberId { get; set; } = string.Empty;
        public string MemberType { get; set; } = string.Empty; // Student, Teacher
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
        public DateTime? ReturnDate { get; set; }
        public decimal FineAmount { get; set; } = 0;
        public string Status { get; set; } = "Issued"; // Issued, Returned, Overdue
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== SUBSCRIPTION PLAN ====================
    public class SubscriptionPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public decimal AnnualPrice { get; set; }
        public string StripePriceIdMonthly { get; set; } = string.Empty;
        public string StripePriceIdAnnual { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int MaxTeachers { get; set; }
        public int MaxAdmins { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== AUDIT LOG ====================
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ==================== HOMEWORK ====================
    public class Homework
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public string AttachmentUrl { get; set; } = string.Empty;
        public List<HomeworkSubmission> Submissions { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class HomeworkSubmission
    {
        public string StudentId { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Status { get; set; } = "Submitted";
        public decimal? Marks { get; set; }
        public string TeacherFeedback { get; set; } = string.Empty;
    }

    // ==================== EVENT ====================
    public class SchoolEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
