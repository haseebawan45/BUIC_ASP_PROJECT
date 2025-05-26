using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public decimal MaxPoints { get; set; }
        
        public ICollection<Submission> Submissions { get; set; }
    }

    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }
        
        public int AssignmentId { get; set; }
        [ForeignKey("AssignmentId")]
        public Assignment Assignment { get; set; }
        
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        
        public string SubmissionContent { get; set; }
        
        public string FilePath { get; set; }
        
        public DateTime SubmissionDate { get; set; }
        
        public decimal? Grade { get; set; }
        
        public string Feedback { get; set; }
    }
} 