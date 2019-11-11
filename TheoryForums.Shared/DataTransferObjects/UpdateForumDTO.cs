using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class UpdateForumDTO
    {
        [Required] public string Title { get; set; }
        public string Description { get; set; }
    }
}
