using System.ComponentModel.DataAnnotations;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class CreateThreadDTO
    {
        [Required]
        [MaxLength(90, ErrorMessage = "Max length is 90 characters")]
        public string Title { get; set; }
        
        [Required] 
        public string Content { get; set; }
        
        [Required] 
        public int SubforumId { get; set; }
    }
}
