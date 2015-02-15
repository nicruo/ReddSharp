using System.Collections.Generic;

namespace Nicruo.ReddSharp.Domain
{
    public class PostComments
    {
        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
    }
}