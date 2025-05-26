using Microsoft.AspNetCore.Identity;

namespace BUIC_ASP_PROJECT.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public UserType UserType { get; set; }
    }

    public enum UserType
    {
        Student,
        Faculty,
        Admin
    }
} 