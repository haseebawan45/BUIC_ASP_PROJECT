using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        public string StudentNumber { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [Required]
        public string Program { get; set; }
        
        public int Semester { get; set; }
        
        public decimal GPA { get; set; }
        
        public ICollection<StudentCourse> EnrolledCourses { get; set; }
        
        public ICollection<Assignment> Assignments { get; set; }
    }
} 