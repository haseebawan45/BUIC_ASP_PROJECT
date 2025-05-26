using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }
        
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public bool IsPresent { get; set; }
        
        public string Remarks { get; set; }
    }
    
    public class AttendanceReport
    {
        [Key]
        public int ReportId { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string Remarks { get; set; }
        
        // Attendance percentage for the course
        public decimal AttendancePercentage { get; set; }
    }
}
