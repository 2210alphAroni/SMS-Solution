using MongoDB.Driver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class StudentService
    {
        private readonly MongoDbContext _context;

        public StudentService(MongoDbContext context) => _context = context;

        public async Task<List<Student>> GetAllAsync(string tenantId, string? classId = null, string? section = null, string? status = null)
        {
            var builder = Builders<Student>.Filter;
            var filter = builder.Eq(s => s.TenantId, tenantId);
            if (!string.IsNullOrEmpty(classId)) filter &= builder.Eq(s => s.ClassId, classId);
            if (!string.IsNullOrEmpty(section)) filter &= builder.Eq(s => s.Section, section);
            if (!string.IsNullOrEmpty(status)) filter &= builder.Eq(s => s.Status, status);
            return await _context.Students.Find(filter).SortBy(s => s.FirstName).ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(string id, string tenantId)
        {
            return await _context.Students.Find(s => s.Id == id && s.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Student?> GetByAdmissionNoAsync(string admissionNo, string tenantId)
        {
            return await _context.Students.Find(s => s.AdmissionNo == admissionNo && s.TenantId == tenantId).FirstOrDefaultAsync();
        }

        public async Task<Student> CreateAsync(Student student)
        {
            student.AdmissionNo = await GenerateAdmissionNoAsync(student.TenantId);
            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;
            await _context.Students.InsertOneAsync(student);
            return student;
        }

        public async Task UpdateAsync(Student student)
        {
            student.UpdatedAt = DateTime.UtcNow;
            await _context.Students.ReplaceOneAsync(s => s.Id == student.Id && s.TenantId == student.TenantId, student);
        }

        public async Task DeleteAsync(string id, string tenantId)
        {
            await _context.Students.DeleteOneAsync(s => s.Id == id && s.TenantId == tenantId);
        }

        public async Task<long> CountAsync(string tenantId, string? status = null)
        {
            var filter = Builders<Student>.Filter.Eq(s => s.TenantId, tenantId);
            if (!string.IsNullOrEmpty(status)) filter &= Builders<Student>.Filter.Eq(s => s.Status, status);
            return await _context.Students.CountDocumentsAsync(filter);
        }

        private async Task<string> GenerateAdmissionNoAsync(string tenantId)
        {
            var count = await _context.Students.CountDocumentsAsync(s => s.TenantId == tenantId);
            return $"ADM{DateTime.UtcNow.Year}{(count + 1):D4}";
        }

        public async Task<List<Student>> SearchAsync(string tenantId, string query)
        {
            var filter = Builders<Student>.Filter.And(
                Builders<Student>.Filter.Eq(s => s.TenantId, tenantId),
                Builders<Student>.Filter.Or(
                    Builders<Student>.Filter.Regex(s => s.FirstName, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<Student>.Filter.Regex(s => s.LastName, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<Student>.Filter.Regex(s => s.AdmissionNo, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<Student>.Filter.Regex(s => s.Email, new MongoDB.Bson.BsonRegularExpression(query, "i"))
                )
            );
            return await _context.Students.Find(filter).Limit(20).ToListAsync();
        }
    }
}
