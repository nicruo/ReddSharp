using System.Collections.Generic;

namespace Nicruo.ReddSharp.Domain
{
    public class Subreddit
    {
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }
}