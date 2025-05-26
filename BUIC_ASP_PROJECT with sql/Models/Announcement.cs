using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty Creator { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public bool IsActive { get; set; }
    }
} 