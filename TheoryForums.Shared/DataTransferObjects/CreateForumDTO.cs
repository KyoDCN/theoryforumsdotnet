using System.ComponentModel.DataAnnotations;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class CreateForumDTO
    {
        [Required] public string Title { get; set; }
        public string Description { get; set; }
    }
}
