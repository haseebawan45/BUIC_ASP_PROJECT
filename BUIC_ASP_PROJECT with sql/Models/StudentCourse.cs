using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        public decimal? Midterm { get; set; }
        public decimal? Final { get; set; }
        public decimal? Assignments { get; set; }
        public decimal? TotalGrade { get; set; }
        public string Grade { get; set; }
        
        public DateTime EnrollmentDate { get; set; }
    }
} 