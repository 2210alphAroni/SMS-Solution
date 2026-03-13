using MongoDB.Driver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class TenantService
    {
        private readonly MongoDbContext _context;

        public TenantService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants.Find(t => t.Subdomain == subdomain).FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetByIdAsync(string id)
        {
            return await _context.Tenants.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Tenant>> GetAllAsync()
        {
            return await _context.Tenants.Find(_ => true).ToListAsync();
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            tenant.CreatedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;
            await _context.Tenants.InsertOneAsync(tenant);
            return tenant;
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            tenant.UpdatedAt = DateTime.UtcNow;
            await _context.Tenants.ReplaceOneAsync(t => t.Id == tenant.Id, tenant);
        }

        public async Task<bool> SubdomainExistsAsync(string subdomain)
        {
            return await _context.Tenants.Find(t => t.Subdomain == subdomain).AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Tenants.Find(t => t.Email == email).AnyAsync();
        }

        public async Task UpdateSubscriptionAsync(string tenantId, string plan, DateTime endDate, string stripeSubId)
        {
            var update = Builders<Tenant>.Update
                .Set(t => t.SubscriptionPlan, plan)
                .Set(t => t.SubscriptionEndDate, endDate)
                .Set(t => t.StripeSubscriptionId, stripeSubId)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);
            await _context.Tenants.UpdateOneAsync(t => t.Id == tenantId, update);
        }

        public async Task<DashboardStats> GetDashboardStatsAsync(string tenantId)
        {
            var studentCount = await _context.Students.CountDocumentsAsync(s => s.TenantId == tenantId && s.Status == "Active");
            var teacherCount = await _context.Teachers.CountDocumentsAsync(t => t.TenantId == tenantId && t.Status == "Active");
            var classCount = await _context.Classes.CountDocumentsAsync(c => c.TenantId == tenantId && c.IsActive);
            var today = DateTime.UtcNow.Date;
            var feeThisMonth = await _context.FeePayments
                .Find(f => f.TenantId == tenantId && f.PaymentDate >= today.AddDays(-today.Day + 1))
                .ToListAsync();
            var totalFeeCollected = feeThisMonth.Sum(f => f.PaidAmount);
            var pendingLeaves = await _context.Leaves.CountDocumentsAsync(l => l.TenantId == tenantId && l.Status == "Pending");
            var recentNotices = await _context.Notices
                .Find(n => n.TenantId == tenantId && n.IsActive)
                .SortByDescending(n => n.CreatedAt)
                .Limit(5)
                .ToListAsync();
            var upcomingEvents = await _context.Events
                .Find(e => e.TenantId == tenantId && e.StartDate >= DateTime.UtcNow)
                .SortBy(e => e.StartDate)
                .Limit(5)
                .ToListAsync();

            return new DashboardStats
            {
                TotalStudents = (int)studentCount,
                TotalTeachers = (int)teacherCount,
                TotalClasses = (int)classCount,
                FeeCollectedThisMonth = totalFeeCollected,
                PendingLeaves = (int)pendingLeaves,
                RecentNotices = recentNotices,
                UpcomingEvents = upcomingEvents
            };
        }
    }

    public class DashboardStats
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public decimal FeeCollectedThisMonth { get; set; }
        public int PendingLeaves { get; set; }
        public List<Notice> RecentNotices { get; set; } = new();
        public List<SchoolEvent> UpcomingEvents { get; set; } = new();
    }
}
