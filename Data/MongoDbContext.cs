using MongoDB.Driver;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            CreateIndexes();
        }

        public IMongoCollection<Tenant> Tenants => _database.GetCollection<Tenant>("Tenants");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<AcademicYear> AcademicYears => _database.GetCollection<AcademicYear>("AcademicYears");
        public IMongoCollection<Class> Classes => _database.GetCollection<Class>("Classes");
        public IMongoCollection<Subject> Subjects => _database.GetCollection<Subject>("Subjects");
        public IMongoCollection<Student> Students => _database.GetCollection<Student>("Students");
        public IMongoCollection<Teacher> Teachers => _database.GetCollection<Teacher>("Teachers");
        public IMongoCollection<Parent> Parents => _database.GetCollection<Parent>("Parents");
        public IMongoCollection<Attendance> Attendances => _database.GetCollection<Attendance>("Attendances");
        public IMongoCollection<Exam> Exams => _database.GetCollection<Exam>("Exams");
        public IMongoCollection<ExamResult> Results => _database.GetCollection<ExamResult>("Results");
        public IMongoCollection<FeeStructure> FeeStructures => _database.GetCollection<FeeStructure>("FeeStructures");
        public IMongoCollection<FeePayment> FeePayments => _database.GetCollection<FeePayment>("FeePayments");
        public IMongoCollection<Notice> Notices => _database.GetCollection<Notice>("Notices");
        public IMongoCollection<Timetable> Timetables => _database.GetCollection<Timetable>("Timetables");
        public IMongoCollection<Leave> Leaves => _database.GetCollection<Leave>("Leaves");
        public IMongoCollection<Book> Books => _database.GetCollection<Book>("Books");
        public IMongoCollection<BookIssue> BookIssues => _database.GetCollection<BookIssue>("BookIssues");
        public IMongoCollection<SubscriptionPlan> SubscriptionPlans => _database.GetCollection<SubscriptionPlan>("SubscriptionPlans");
        public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");
        public IMongoCollection<Homework> Homeworks => _database.GetCollection<Homework>("Homeworks");
        public IMongoCollection<SchoolEvent> Events => _database.GetCollection<SchoolEvent>("Events");

        private void CreateIndexes()
        {
            Tenants.Indexes.CreateOne(new CreateIndexModel<Tenant>(
                Builders<Tenant>.IndexKeys.Ascending(t => t.Subdomain),
                new CreateIndexOptions { Unique = true }));

            Users.Indexes.CreateOne(new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Combine(
                    Builders<User>.IndexKeys.Ascending(u => u.TenantId),
                    Builders<User>.IndexKeys.Ascending(u => u.Email)),
                new CreateIndexOptions { Unique = true }));

            Students.Indexes.CreateOne(new CreateIndexModel<Student>(
                Builders<Student>.IndexKeys.Combine(
                    Builders<Student>.IndexKeys.Ascending(s => s.TenantId),
                    Builders<Student>.IndexKeys.Ascending(s => s.AdmissionNo)),
                new CreateIndexOptions { Unique = true }));

            Attendances.Indexes.CreateOne(new CreateIndexModel<Attendance>(
                Builders<Attendance>.IndexKeys.Combine(
                    Builders<Attendance>.IndexKeys.Ascending(a => a.TenantId),
                    Builders<Attendance>.IndexKeys.Ascending(a => a.ClassId),
                    Builders<Attendance>.IndexKeys.Ascending(a => a.Date))));
        }
    }
}
