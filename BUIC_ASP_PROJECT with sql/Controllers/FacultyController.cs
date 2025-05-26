using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BUIC_ASP_PROJECT.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BUIC_ASP_PROJECT.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.Faculty
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            var courses = await _context.Courses
                .Where(c => c.FacultyId == faculty.FacultyId)
                .ToListAsync();

            ViewBag.Courses = courses;
            
            return View(faculty);
        }

        // Course Management
        public async Task<IActionResult> Courses()
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.Faculty.FirstOrDefaultAsync(f => f.UserId == user.Id);

            var courses = await _context.Courses
                .Where(c => c.FacultyId == faculty.FacultyId)
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                        .ThenInclude(s => s.User)
                .ToListAsync();

            return View(courses);
        }

        // View specific course
        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _context.Courses
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                        .ThenInclude(s => s.User)
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // Create assignment
        [HttpGet]
        public IActionResult CreateAssignment(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CourseDetails), new { id = assignment.CourseId });
            }

            ViewBag.CourseId = assignment.CourseId;
            return View(assignment);
        }

        // Grade submissions
        public async Task<IActionResult> Submissions(int assignmentId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // Grade a submission
        [HttpGet]
        public async Task<IActionResult> GradeSubmission(int submissionId)
        {
            var submission = await _context.Submissions
                .Include(s => s.Student)
                    .ThenInclude(s => s.User)
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        [HttpPost]
        public async Task<IActionResult> GradeSubmission(int submissionId, decimal grade, string feedback)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (submission == null)
            {
                return NotFound();
            }

            submission.Grade = grade;
            submission.Feedback = feedback;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Submissions), new { assignmentId = submission.AssignmentId });
        }

        // Create announcement
        [HttpGet]
        public IActionResult CreateAnnouncement(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var faculty = await _context.Faculty.FirstOrDefaultAsync(f => f.UserId == user.Id);
                
                announcement.FacultyId = faculty.FacultyId;
                announcement.CreatedAt = DateTime.Now;
                announcement.IsActive = true;
                
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(CourseDetails), new { id = announcement.CourseId });
            }

            ViewBag.CourseId = announcement.CourseId;
            return View(announcement);
        }

        // View attendance
        public async Task<IActionResult> Attendance(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Get attendance records for this course
            var attendanceRecords = await _context.Attendances
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
            
            ViewBag.AttendanceRecords = attendanceRecords;
            ViewBag.Today = DateTime.Today;

            return View(course);
        }
        
        // Take attendance
        [HttpGet]
        public async Task<IActionResult> TakeAttendance(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Check if attendance has already been taken today
            var today = DateTime.Today;
            var existingAttendance = await _context.Attendances
                .Where(a => a.CourseId == courseId && a.Date.Date == today)
                .ToListAsync();
            
            if (existingAttendance.Any())
            {
                ViewBag.AlreadyTaken = true;
                ViewBag.ExistingAttendance = existingAttendance;
            }

            return View(course);
        }
        
        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int courseId, List<int> presentStudentIds)
        {
            var course = await _context.Courses
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Get all students enrolled in this course
            var enrolledStudents = course.EnrolledStudents.Select(sc => sc.Student).ToList();
        
            // Clear any existing attendance records for today
            var today = DateTime.Today;
            var existingAttendance = await _context.Attendances
                .Where(a => a.CourseId == courseId && a.Date.Date == today)
                .ToListAsync();
            
            if (existingAttendance.Any())
            {
                _context.Attendances.RemoveRange(existingAttendance);
            }
        
            // Create new attendance records
            foreach (var student in enrolledStudents)
            {
                var attendance = new Attendance
                {
                    StudentId = student.StudentId,
                    CourseId = courseId,
                    Date = today,
                    IsPresent = presentStudentIds.Contains(student.StudentId),
                    Remarks = ""
                };
            
                _context.Attendances.Add(attendance);
            }
        
            await _context.SaveChangesAsync();
        
            return RedirectToAction(nameof(Attendance), new { courseId });
        }
        
        // Generate attendance report
        public async Task<IActionResult> AttendanceReport(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.EnrolledStudents)
                    .ThenInclude(sc => sc.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }
        
            // Get all attendance records for this course
            var attendanceRecords = await _context.Attendances
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .OrderBy(a => a.Date)
                .ToListAsync();
            
            // Group by student to calculate attendance percentage
            var studentAttendance = attendanceRecords
                .GroupBy(a => a.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    StudentName = g.First().Student.User.FirstName + " " + g.First().Student.User.LastName,
                    TotalClasses = g.Count(),
                    PresentCount = g.Count(a => a.IsPresent),
                    AttendancePercentage = g.Count() > 0 ? (decimal)g.Count(a => a.IsPresent) / g.Count() * 100 : 0
                })
                .OrderByDescending(s => s.AttendancePercentage)
                .ToList();
            
            ViewBag.StudentAttendance = studentAttendance;
            ViewBag.TotalClasses = attendanceRecords.Select(a => a.Date.Date).Distinct().Count();
        
            return View(course);
        }
    }
}