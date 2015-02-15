using System.Collections.Generic;

namespace Nicruo.ReddSharp.Domain
{
    public class Comment
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public long Score { get; set; }
        public List<Comment> Replies { get; set; }
    }
}