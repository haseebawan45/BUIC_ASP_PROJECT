using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        
        [Required]
        public string CourseCode { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public int CreditHours { get; set; }
        
        [ForeignKey("FacultyId")]
        public int? FacultyId { get; set; }
        public Faculty Instructor { get; set; }
        
        public string Semester { get; set; }
        
        public ICollection<StudentCourse> EnrolledStudents { get; set; }
        
        public ICollection<Assignment> Assignments { get; set; }
        
        public ICollection<Course> Prerequisites { get; set; }
    }
} 