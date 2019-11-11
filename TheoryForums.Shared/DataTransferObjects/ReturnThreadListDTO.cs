using System;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnThreadListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastReplyDate { get; set; }
        public long Views { get; set; }
        public int Replies { get; set; }
        public bool Edited { get; set; }
    }
}
