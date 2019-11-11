using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class UpdateThreadDTO
    {
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }
    }
}
