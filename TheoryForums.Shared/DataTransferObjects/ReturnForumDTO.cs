using System;
using System.Collections.Generic;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnForumDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public IEnumerable<ReturnSubforumDTO> Subforums { get; set; }
    }
}
