using System;
using System.Collections.Generic;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class ReturnSubforumDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public LatestReply LatestReply { get; set; }
    }
}