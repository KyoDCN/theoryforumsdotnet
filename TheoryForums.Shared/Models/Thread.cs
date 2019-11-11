using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheorySlugify;

namespace TheoryForums.Shared.Models
{
    public class Thread
    {
        private string _Title;

        public int Id { get; set; }
        [Required] public string Title { get => _Title; set { _Title = value; Slug = value.GenerateSlug(); }}
        [Required] public string Slug { get; set; }
        [Required] public string Content { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public bool Edited { get; set; } = false;
        public DateTime LastEditDate { get; set; } = DateTime.MinValue;
        public long Views { get; set; }

        // Post
        public virtual ICollection<Post> Posts { get; set; }

        // Subforum
        public int SubforumFK { get; set; }
        public virtual Subforum Subforum { get; set; }

        // User
        public int AuthorId { get; set; }
    }
}
