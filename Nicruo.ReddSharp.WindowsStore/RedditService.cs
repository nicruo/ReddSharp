using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicruo.ReddSharp;
using Nicruo.ReddSharp.Domain;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Nicruo.ReddSharp.WindowsStore
{
    public class RedditService: IRedditService
    {
        HttpClient httpClient;

        public RedditService()
        {
            httpClient = new HttpClient();
        }

        public async Task<IList<string>> GetSubredditsAsync()
        {
            var subreddits = new List<string>();

            var response = await httpClient.GetStringAsync("http://www.reddit.com/.json");

            var jObject = JObject.Parse(response);

            var jArray = jObject["data"]["children"] as JArray;

            foreach (var obj in jArray)
            {
                subreddits.Add(obj["data"]["subreddit"].ToString());
            }

            subreddits = subreddits.OrderBy(sr => sr).ToList();

            return subreddits;
        }

        public async Task<Subreddit> GetSubredditAsync(string subredditName)
        {
            var subreddit = new Subreddit();

            subreddit.Name = subredditName;

            subreddit.Posts = new List<Post>();

            var response = await httpClient.GetStringAsync("http://www.reddit.com/r/" + subredditName + "/.json");

            var jObject = JObject.Parse(response);

            var jArray = jObject["data"]["children"] as JArray;

            foreach (var obj in jArray)
            {
                var post = new Post();

                post.Title = obj["data"]["title"].ToString();
                post.Author = obj["data"]["author"].ToString();
                post.Score = obj["data"]["score"].Value<int>();
                post.Thumbnail = obj["data"]["thumbnail"].ToString();
                post.Url = obj["data"]["url"].ToString();
                post.NumberOfComments = obj["data"]["num_comments"].Value<int>();

                subreddit.Posts.Add(post);
            }

            return subreddit;
        }

        public async Task<SubredditAbout> GetSubredditAboutAsync(string subredditName)
        {
            var subredditAbout = new SubredditAbout();

            var response = await httpClient.GetStringAsync("http://www.reddit.com/r/" + subredditName + "/about.json");

            var jObject = JObject.Parse(response);

            var obj = jObject["data"];

            subredditAbout.SubmitText = obj["submit_text"].ToString();
            subredditAbout.DisplayName = obj["display_name"].ToString();
            subredditAbout.HeaderImg = obj["header_img"].ToString();
            subredditAbout.DescriptionHtml = obj["description_html"].ToString();
            subredditAbout.Title = obj["title"].ToString();
            subredditAbout.Over18 = obj["over18"].Value<bool>();
            subredditAbout.Description = obj["description"].ToString();
            subredditAbout.Subscribers = obj["subscribers"].Value<long>();
            subredditAbout.PublicDescription = obj["public_description"].ToString();

            return subredditAbout;
        }

        public async Task<IList<string>> SearchForSubredditsAsync(string query)
        {
            var subreddits = new List<string>();

            var response = await httpClient.GetStringAsync("http://www.reddit.com/subreddits/search.json?q=" + query);

            var jObject = JObject.Parse(response);

            var jArray = jObject["data"]["children"] as JArray;

            foreach (var obj in jArray)
            {
                subreddits.Add(obj["data"]["display_name"].ToString());
            }

            return subreddits;
        }
    }
}
