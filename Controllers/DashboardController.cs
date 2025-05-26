using Microsoft.AspNetCore.Mvc;

namespace BUIC_ASP_PROJECT.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index(string username)
        {
            // If no username is provided, default to "Guest"
            if (string.IsNullOrEmpty(username))
            {
                username = "Guest";
            }
            
            // Store username in TempData to be available across requests
            TempData["Username"] = username;
            ViewBag.Username = username;
            
            return View();
        }
        
        public IActionResult Profile()
        {
            // Preserve the username for subsequent requests
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult Courses()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult Grades()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult Attendance()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult Lectures()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult Assignments()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult RegisterCourses()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
        
        public IActionResult FeeDetails()
        {
            ViewBag.Username = TempData["Username"];
            TempData.Keep("Username");
            return View();
        }
    }
} 