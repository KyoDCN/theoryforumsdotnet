using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class CreatePostDTO
    {
        [Required] public dynamic Content { get; set; }
        [Required] public int ThreadId { get; set; }
    }
}
