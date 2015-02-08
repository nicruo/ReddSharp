using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicruo.ReddSharp.Domain
{
    public class Post
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public string Thumbnail { get; set; }
        public string Url { get; set; }
        public int NumberOfComments { get; set; }
    }
}