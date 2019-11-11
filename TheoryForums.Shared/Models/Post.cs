using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TheoryForums.Shared.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required] public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Edited { get; set; }
        public DateTime LastEditDate { get; set; }

        // Thread
        public int ThreadFK { get; set; }
        public virtual Thread Thread { get; set; }

        // User
        public int AuthorId { get; set; }
    }
}
