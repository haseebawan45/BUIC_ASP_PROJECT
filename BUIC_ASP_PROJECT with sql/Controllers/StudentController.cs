using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BUIC_ASP_PROJECT.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BUIC_ASP_PROJECT.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            return View(student);
        }

        // View academic record
        public async Task<IActionResult> AcademicRecord()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.EnrolledCourses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            return View(student);
        }

        // Course registration
        public async Task<IActionResult> CourseRegistration()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            var availableCourses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.EnrolledStudents.Where(sc => sc.StudentId == student.StudentId))
                .ToListAsync();

            return View(availableCourses);
        }

        // Register for course
        [HttpPost]
        public async Task<IActionResult> RegisterCourse(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            // Check if already registered
            var existingRegistration = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == student.StudentId && sc.CourseId == courseId);

            if (existingRegistration == null)
            {
                var newRegistration = new StudentCourse
                {
                    StudentId = student.StudentId,
                    CourseId = courseId,
                    EnrollmentDate = DateTime.Now
                };

                _context.StudentCourses.Add(newRegistration);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CourseRegistration));
        }

        // View assignments
        public async Task<IActionResult> Assignments()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            
            var enrolledCourses = await _context.StudentCourses
                .Where(sc => sc.StudentId == student.StudentId)
                .Select(sc => sc.CourseId)
                .ToListAsync();

            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Submissions.Where(s => s.StudentId == student.StudentId))
                .Where(a => enrolledCourses.Contains(a.CourseId))
                .ToListAsync();

            return View(assignments);
        }

        // Submit assignment
        [HttpGet]
        public async Task<IActionResult> SubmitAssignment(int assignmentId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(new SubmissionViewModel { AssignmentId = assignmentId, Assignment = assignment });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(SubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

                var submission = new Submission
                {
                    AssignmentId = model.AssignmentId,
                    StudentId = student.StudentId,
                    SubmissionContent = model.Content,
                    SubmissionDate = DateTime.Now
                };

                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Assignments));
            }

            return View(model);
        }

        // View grades
        public async Task<IActionResult> Grades()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            var grades = await _context.StudentCourses
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == student.StudentId)
                .ToListAsync();

            return View(grades);
        }

        // View fees
        public async Task<IActionResult> Fees()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            var fees = await _context.Fees
                .Where(f => f.StudentId == student.StudentId)
                .ToListAsync();

            return View(fees);
        }
    }

    public class SubmissionViewModel
    {
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public string Content { get; set; }
        public IFormFile File { get; set; }
    }
} 