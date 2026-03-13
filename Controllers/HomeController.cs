using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly MongoDbContext _db;

        public HomeController(TenantService tenantService, Data.MongoDbContext db)
        {
            _tenantService = tenantService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            var plans = await _db.SubscriptionPlans.Find(_ => true).ToListAsync();
            return View(plans);
        }

        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Pricing() => View();
        public IActionResult Features() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
