using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BUIC_ASP_PROJECT.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BUIC_ASP_PROJECT.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard statistics
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.FacultyCount = await _context.Faculty.CountAsync();
            ViewBag.CourseCount = await _context.Courses.CountAsync();

            return View();
        }

        // Users Management
        public async Task<IActionResult> UserManagement()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .ToListAsync();

            var faculty = await _context.Faculty
                .Include(f => f.User)
                .ToListAsync();

            var viewModel = new UserManagementViewModel
            {
                Students = students,
                FacultyMembers = faculty
            };

            return View(viewModel);
        }

        // Course Management
        public async Task<IActionResult> CourseManagement()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();

            return View(courses);
        }

        // Fee Reports
        public async Task<IActionResult> FeeReports()
        {
            var fees = await _context.Fees
                .Include(f => f.Student)
                .Include(f => f.Student.User)
                .ToListAsync();

            return View(fees);
        }

        // Registration Reports
        public async Task<IActionResult> RegistrationReports()
        {
            var registrations = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Student.User)
                .Include(sc => sc.Course)
                .ToListAsync();

            return View(registrations);
        }
    }

    public class UserManagementViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Faculty> FacultyMembers { get; set; }
    }
} 