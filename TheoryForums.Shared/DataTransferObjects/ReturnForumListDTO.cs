using System;
using System.Collections.Generic;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnForumListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public List<Subforums> Subforums { get; set; }
    }

    public class Subforums
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public LatestReply LatestReply { get; set; }
    }

    public class LatestReply
    {
        public int ThreadId { get; set; }
        public string ThreadTitle { get; set; }
        public string ThreadSlug { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime PostReplyDate { get; set; }
    }
}
