using System.ComponentModel.DataAnnotations;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class UpdatePostDTO
    {
        [Required] public dynamic Content { get; set; }
    }
}
