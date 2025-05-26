using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Faculty
    {
        [Key]
        public int FacultyId { get; set; }
        
        [Required]
        public string FacultyNumber { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [Required]
        public string Department { get; set; }
        
        public string Designation { get; set; }
        
        public ICollection<Course> Courses { get; set; }
    }
} 