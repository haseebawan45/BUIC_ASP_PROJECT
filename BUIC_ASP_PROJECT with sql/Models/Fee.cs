using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BUIC_ASP_PROJECT.Models
{
    public class Fee
    {
        [Key]
        public int FeeId { get; set; }
        
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        
        [Required]
        public string Semester { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public bool IsPaid { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        public string TransactionReference { get; set; }
    }
} 