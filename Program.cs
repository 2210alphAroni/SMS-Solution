using Microsoft.AspNetCore.Authentication.Cookies;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Middleware;
using SchoolManagementSystem.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// MongoDB
builder.Services.AddSingleton<MongoDbContext>();

// Services
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<FeeService>();
builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<NoticeService>();
builder.Services.AddScoped<AcademicYearService>();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", p => p.RequireRole("SuperAdmin"));
    options.AddPolicy("Admin", p => p.RequireRole("SuperAdmin", "Admin"));
    options.AddPolicy("Teacher", p => p.RequireRole("SuperAdmin", "Admin", "Teacher"));
    options.AddPolicy("Student", p => p.RequireRole("SuperAdmin", "Admin", "Teacher", "Student"));
});

// MVC
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSerilogRequestLogging();

// Multi-tenancy middleware
app.UseTenantMiddleware();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Route configuration
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed super admin
await SeedData.InitializeAsync(app.Services);

app.Run();

// Seed initial data
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Check if super admin exists
        var superAdmin = await db.Users.Find(u => u.Role == "SuperAdmin").FirstOrDefaultAsync();
        if (superAdmin != null) return;

        // Create platform tenant
        var platformTenant = new SchoolManagementSystem.Models.Tenant
        {
            SchoolName = "EduManage Pro Platform",
            Subdomain = "platform",
            Email = "admin@edumanagepro.com",
            SubscriptionPlan = "Enterprise",
            IsActive = true,
            IsEmailVerified = true,
            SubscriptionEndDate = DateTime.UtcNow.AddYears(100)
        };
        await db.Tenants.InsertOneAsync(platformTenant);

        // Create super admin user
        var adminUser = new SchoolManagementSystem.Models.User
        {
            TenantId = platformTenant.Id!,
            Email = "admin@edumanagepro.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
            FirstName = "Super",
            LastName = "Admin",
            Role = "SuperAdmin",
            IsActive = true,
            IsEmailVerified = true
        };
        await db.Users.InsertOneAsync(adminUser);

        // Seed subscription plans
        var plans = new List<SchoolManagementSystem.Models.SubscriptionPlan>
        {
            new() {
                Name = "Free",
                Description = "Perfect for small schools getting started",
                MonthlyPrice = 0,
                AnnualPrice = 0,
                MaxStudents = 50,
                MaxTeachers = 5,
                MaxAdmins = 1,
                Features = new() { "Student Management", "Attendance", "Basic Reports", "1 Admin Account" }
            },
            new() {
                Name = "Basic",
                Description = "For growing schools",
                MonthlyPrice = 999,
                AnnualPrice = 9990,
                MaxStudents = 300,
                MaxTeachers = 30,
                MaxAdmins = 3,
                Features = new() { "All Free Features", "Exam & Results", "Fee Management", "Notice Board", "Parent Portal", "3 Admin Accounts" }
            },
            new() {
                Name = "Professional",
                Description = "For established schools",
                MonthlyPrice = 2499,
                AnnualPrice = 24990,
                MaxStudents = 1000,
                MaxTeachers = 100,
                MaxAdmins = 10,
                Features = new() { "All Basic Features", "Library Management", "Timetable", "Leave Management", "SMS Notifications", "Advanced Reports", "Online Fee Payment" }
            },
            new() {
                Name = "Enterprise",
                Description = "For large school networks",
                MonthlyPrice = 4999,
                AnnualPrice = 49990,
                MaxStudents = 999999,
                MaxTeachers = 999999,
                MaxAdmins = 999999,
                Features = new() { "All Professional Features", "Unlimited Students & Teachers", "Multi-Branch Support", "Custom Domain", "Priority Support", "API Access", "Custom Integrations" }
            }
        };
        await db.SubscriptionPlans.InsertManyAsync(plans);

        Console.WriteLine("✅ Seed data initialized. Login: admin@edumanagepro.com / Admin@123456");
    }
}
