using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BUIC_ASP_PROJECT.Models;
using System.Threading.Tasks;

namespace BUIC_ASP_PROJECT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (roles.Contains("Faculty"))
                    {
                        return RedirectToAction("Index", "Faculty");
                    }
                    else if (roles.Contains("Student"))
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate required fields based on user type
                    if (model.UserType == UserType.Faculty && string.IsNullOrEmpty(model.Department))
                    {
                        ModelState.AddModelError("Department", "Department is required for Faculty.");
                        return View(model);
                    }
                    
                    if (model.UserType == UserType.Student && string.IsNullOrEmpty(model.Program))
                    {
                        ModelState.AddModelError("Program", "Program is required for Students.");
                        return View(model);
                    }
                    
                    if ((model.UserType == UserType.Faculty || model.UserType == UserType.Student) && 
                        string.IsNullOrEmpty(model.UserNumber))
                    {
                        ModelState.AddModelError("UserNumber", "User Number is required.");
                        return View(model);
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserType = model.UserType,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        bool roleAdded = false;
                        bool relatedEntityAdded = true; // Assume success unless otherwise noted
                        
                        // Assign role based on user type
                        switch (model.UserType)
                        {
                            case UserType.Admin:
                                var adminRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
                                roleAdded = adminRoleResult.Succeeded;
                                break;
                                
                            case UserType.Faculty:
                                var facultyRoleResult = await _userManager.AddToRoleAsync(user, "Faculty");
                                roleAdded = facultyRoleResult.Succeeded;
                                
                                // Create faculty record
                                var faculty = new Faculty
                                {
                                    FacultyNumber = model.UserNumber,
                                    UserId = user.Id,
                                    Department = model.Department,
                                    Designation = "Lecturer" // Default designation
                                };
                                
                                // Add faculty to database
                                _context.Faculty.Add(faculty);
                                var facultySaveResult = await _context.SaveChangesAsync();
                                relatedEntityAdded = facultySaveResult > 0;
                                break;
                                
                            case UserType.Student:
                                var studentRoleResult = await _userManager.AddToRoleAsync(user, "Student");
                                roleAdded = studentRoleResult.Succeeded;
                                
                                // Create student record
                                var student = new Student
                                {
                                    StudentNumber = model.UserNumber,
                                    UserId = user.Id,
                                    Program = model.Program,
                                    Semester = 1, // Default to first semester
                                    GPA = 0.0m // Default GPA
                                };
                                
                                // Add student to database
                                _context.Students.Add(student);
                                var studentSaveResult = await _context.SaveChangesAsync();
                                relatedEntityAdded = studentSaveResult > 0;
                                break;
                        }

                        if (roleAdded && relatedEntityAdded)
                        {
                            TempData["SuccessMessage"] = $"{model.FirstName} {model.LastName} has been registered successfully as {model.UserType}.";
                            return RedirectToAction("UserManagement", "Admin");
                        }
                        else
                        {
                            // Something went wrong with role or related entity - delete the user
                            await _userManager.DeleteAsync(user);
                            ModelState.AddModelError(string.Empty, "Failed to add role or create related record. User creation has been rolled back.");
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }
    }
} 