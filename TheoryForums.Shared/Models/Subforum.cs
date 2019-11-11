using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheorySlugify;

namespace TheoryForums.Shared.Models
{
    public class Subforum
    {
        private string _Title;

        public int Id { get; set; }
        [Required] public string Title { get => _Title; set { _Title = value; Slug = value.GenerateSlug(); } }
        [Required] public string Slug { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<Thread> Threads { get; set; }

        public int ForumFK { get; set; }
        public virtual Forum Forum { get; set; }
    }
}
