using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnThreadDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Edited { get; set; }
        public DateTime LastEditDate { get; set; }
        public ReturnSubforumDTO Subforum { get; set; }
        public AuthorDTO Author { get; set; }
    }
}
