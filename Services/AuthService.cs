using MongoDB.Driver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SchoolManagementSystem.Services
{
    public class AuthService
    {
        private readonly MongoDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> AuthenticateAsync(string tenantId, string email, string password)
        {
            var user = await _context.Users.Find(u => u.TenantId == tenantId && u.Email == email.ToLower() && u.IsActive).FirstOrDefaultAsync();
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
            var update = Builders<User>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
            await _context.Users.UpdateOneAsync(u => u.Id == user.Id, update);
            return user;
        }

        public async Task SignInAsync(User user, Tenant tenant, bool rememberMe = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("TenantId", user.TenantId),
                new Claim("TenantName", tenant.SchoolName),
                new Claim("Subdomain", tenant.Subdomain),
                new Claim("ProfilePicture", user.ProfilePicture ?? ""),
                new Claim("SubscriptionPlan", tenant.SubscriptionPlan)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<User> CreateUserAsync(string tenantId, string email, string password, string firstName, string lastName, string role)
        {
            var user = new User
            {
                TenantId = tenantId,
                Email = email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) return false;
            var hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var update = Builders<User>.Update.Set(u => u.PasswordHash, hash).Set(u => u.UpdatedAt, DateTime.UtcNow);
            await _context.Users.UpdateOneAsync(u => u.Id == userId, update);
            return true;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersByTenantAsync(string tenantId)
        {
            return await _context.Users.Find(u => u.TenantId == tenantId).ToListAsync();
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }

        public string GetCurrentTenantId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue("TenantId") ?? "";
        }

        public string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "";
        }
    }
}
