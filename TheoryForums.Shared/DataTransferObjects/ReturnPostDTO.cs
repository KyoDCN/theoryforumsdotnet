using System;
using System.Collections.Generic;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnPostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Edited { get; set; }
        public DateTime LastEditDate { get; set; }
        public AuthorDTO Author { get; set; }
    }
}
