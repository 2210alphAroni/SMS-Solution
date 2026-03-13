# 🎓 EduManage Pro — School Management System (SaaS)

A full-featured, multi-tenant **SaaS School Management System** built with **.NET 8 MVC** and **MongoDB Atlas**. Designed specifically for Bangladeshi schools, supporting multiple schools on a single platform via subdomain-based multi-tenancy.

---

## 🚀 Features

### 📚 Academic Management
- **Student Management** — Registration, profiles, admission numbers, document uploads
- **Teacher Management** — Employee records, qualifications, salary tracking
- **Class & Section Management** — Multiple sections per class, class teacher assignment
- **Subject Management** — Subject assignment to classes and teachers
- **Academic Year Management** — Multi-year support with current year tracking

### 📋 Daily Operations
- **Attendance System** — Class-wise daily attendance with Present/Absent/Late/Leave tracking
- **Exam & Results** — Exam scheduling, mark entry, auto grade calculation, rank generation
- **Fee Management** — Flexible fee structures, payment collection, receipt generation
- **Timetable** — Period-wise schedule builder for each class/section
- **Homework** — Teacher can assign and students can submit homework

### 📢 Communication
- **Notice Board** — Pinned & categorized announcements (General, Exam, Holiday, Event, Emergency)
- **Leave Management** — Staff & student leave applications with approval workflow
- **Events Calendar** — School event tracking

### 📊 Reports & Analytics
- Student reports (class-wise, status-wise)
- Fee collection reports with date range filtering
- Attendance reports (class-wise & individual)
- Exam results analysis

### ☁️ SaaS / Multi-Tenancy
- Subdomain-based isolation: `school.edumanagepro.com`
- 4 subscription tiers: Free → Basic → Professional → Enterprise
- Per-tenant data isolation in MongoDB
- Stripe payment integration (subscription billing)

### 🔐 Security
- Cookie-based authentication with roles (SuperAdmin, Admin, Teacher, Student, Parent)
- BCrypt password hashing
- Anti-forgery token protection
- HTTPS-only cookies

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8.0 MVC |
| Database | MongoDB Atlas |
| ORM | MongoDB.Driver |
| Auth | ASP.NET Cookie Authentication |
| Password Hashing | BCrypt.Net-Next |
| Payments | Stripe.net |
| Logging | Serilog |
| Mapping | AutoMapper |
| Excel Export | ClosedXML |
| PDF | iTextSharp |
| UI | Bootstrap 5.3 + Font Awesome 6 |

---

## ⚙️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB Atlas](https://www.mongodb.com/cloud/atlas) account (or local MongoDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

---

## 📦 Setup & Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourorg/school-management-system.git
cd school-management-system
```

### 2. Configure MongoDB Atlas

1. Create a free cluster at [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Create a database user
3. Whitelist your IP address
4. Copy the connection string

### 3. Update `appsettings.json`

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb+srv://USERNAME:PASSWORD@cluster.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "SchoolManagementDB"
  },
  "Stripe": {
    "PublishableKey": "pk_test_YOUR_KEY",
    "SecretKey": "sk_test_YOUR_KEY"
  }
}
```

### 4. Restore Dependencies & Run

```bash
dotnet restore
dotnet run
```

### 5. Open Browser

```
https://localhost:5001
```

### 6. Default Super Admin Login

After first run, seed data creates:
- **Email:** `admin@edumanagepro.com`
- **Password:** `Admin@123456`
- **School Code:** `platform`

---

## 📁 Project Structure

```
SchoolManagementSystem/
├── Controllers/
│   ├── HomeController.cs          # Landing page
│   ├── AccountController.cs       # Auth (login, register, logout)
│   └── Controllers.cs             # Dashboard, Students, Teachers, Classes,
│                                  # Attendance, Exams, Fees, Notices, Reports
├── Models/
│   └── Models.cs                  # All MongoDB document models
├── Services/
│   ├── AuthService.cs             # Authentication & user management
│   ├── TenantService.cs           # Multi-tenancy & dashboard stats
│   └── Services.cs                # Student, Teacher, Class, Subject,
│                                  # Attendance, Fee, Exam, Notice services
├── Data/
│   └── MongoDbContext.cs          # MongoDB collections & indexes
├── Middleware/
│   └── TenantMiddleware.cs        # Subdomain-based tenant resolution
├── ViewModels/
│   └── ViewModels.cs              # Login, Register, ChangePassword VMs
├── Views/
│   ├── Shared/_Layout.cshtml      # Main app layout with sidebar
│   ├── Home/Index.cshtml          # Marketing landing page
│   ├── Account/                   # Login, Register, AccessDenied
│   ├── Dashboard/Index.cshtml     # Admin dashboard
│   ├── Students/                  # Index, Create, Edit, Details
│   ├── Teachers/                  # Index, Create, Edit, Details
│   ├── Classes/                   # Index, Create, Edit
│   ├── Attendance/                # Index, TakeAttendance, Report
│   ├── Exams/                     # Index, Create, EnterResults
│   ├── Fees/                      # Index, Collect, Receipt, Structures
│   ├── Notices/                   # Index, Create, Edit
│   └── Reports/                   # Index, Students, FeeCollection
├── wwwroot/
│   ├── css/site.css               # Custom styles (Bootstrap + custom)
│   └── js/site.js                 # Interactive JS
├── Program.cs                     # App startup & seed data
├── appsettings.json               # Configuration
└── SchoolManagementSystem.csproj  # NuGet packages
```

---

## 🌐 Multi-Tenancy Architecture

Each school registers with a unique **subdomain**:

```
greenvalley.edumanagepro.com  → School: Green Valley High School
sunriseschool.edumanagepro.com → School: Sunrise School
```

The `TenantMiddleware` resolves the tenant from the subdomain on every request. All data (students, teachers, fees, etc.) is scoped to `TenantId`.

**For development**, use the `?tenant=subdomain` query parameter:
```
http://localhost:5000?tenant=greenvalley
```

---

## 💳 Subscription Plans

| Plan | Price/month | Students | Teachers |
|------|------------|----------|----------|
| Free | ৳0 | 50 | 5 |
| Basic | ৳999 | 300 | 30 |
| Professional | ৳2,499 | 1,000 | 100 |
| Enterprise | ৳4,999 | Unlimited | Unlimited |

---

## 🚀 Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SchoolManagementSystem.dll"]
```

### Environment Variables for Production

```bash
MongoDB__ConnectionString=mongodb+srv://...
MongoDB__DatabaseName=SchoolManagementDB
Stripe__SecretKey=sk_live_...
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 📞 Support

- **Email:** support@edumanagepro.com
- **Website:** https://edumanagepro.com
- **Documentation:** https://docs.edumanagepro.com

---

Made with ❤️ in Bangladesh 🇧🇩
