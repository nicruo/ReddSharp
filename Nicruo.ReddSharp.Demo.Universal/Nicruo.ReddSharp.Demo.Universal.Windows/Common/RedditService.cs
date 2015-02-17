using Newtonsoft.Json.Linq;
using Nicruo.ReddSharp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Nicruo.ReddSharp.Demo.Universal.Common
{
    public class RedditService: IRedditService
    {
        #region Singleton Pattern Implementation
        private static volatile RedditService instance;
        private static object syncRoot = new Object();

        public static RedditService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new RedditService();
                    }
                }
                return instance;
            }
        }
        #endregion

        HttpClient httpClient;
        string accessToken;

        public string RedditApiUrl { get { return accessToken != null ? "https://oauth.reddit.com/" : "http://www.reddit.com/"; } }

        public RedditService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent","PSBattle");
        }

        public async Task<IList<string>> GetSubredditsAsync()
        {
            var subreddits = new List<string>();

            var response = await httpClient.GetStringAsync(RedditApiUrl + ".json");

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

            var response = await httpClient.GetStringAsync(RedditApiUrl + "r/" + subredditName + "/.json");

            var jObject = JObject.Parse(response);

            var jArray = jObject["data"]["children"] as JArray;

            foreach (var obj in jArray)
            {
                var post = new Post();

                post.Id = obj["data"]["id"].ToString();
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

            var response = await httpClient.GetStringAsync(RedditApiUrl + "r/" + subredditName + "/about.json");

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

            var response = await httpClient.GetStringAsync(RedditApiUrl + "subreddits/search.json?q=" + query);

            var jObject = JObject.Parse(response);

            var jArray = jObject["data"]["children"] as JArray;

            foreach (var obj in jArray)
            {
                subreddits.Add(obj["data"]["display_name"].ToString());
            }

            return subreddits;
        }

        public async Task<PostComments> GetPostCommentsAsync(string id)
        {
            var postComments = new PostComments();

            var response = await httpClient.GetStringAsync(RedditApiUrl + "comments/" + id + "/.json");

            var jArray = JArray.Parse(response);

            var post = new Post();

            post.Id = jArray[0]["data"]["children"][0]["data"]["id"].ToString();
            post.Title = jArray[0]["data"]["children"][0]["data"]["title"].ToString();
            post.Author = jArray[0]["data"]["children"][0]["data"]["author"].ToString();
            post.Score = jArray[0]["data"]["children"][0]["data"]["score"].Value<int>();
            post.Thumbnail = jArray[0]["data"]["children"][0]["data"]["thumbnail"].ToString();
            post.Url = jArray[0]["data"]["children"][0]["data"]["url"].ToString();
            post.NumberOfComments = jArray[0]["data"]["children"][0]["data"]["num_comments"].Value<int>();

            postComments.Post = post;

            if (jArray.Count > 1)
            {
                var comments = ParseComments(jArray[1] as JObject);
                postComments.Comments = comments;
            }
            return postComments;
        }

        private List<Comment> ParseComments (JObject obj)
        {
            if (obj == null)
                return null;
            
            var comments = new List<Comment>();
            foreach(var ob in obj["data"]["children"])
            {
                if (ob["kind"].ToString() != "t1")
                    continue;
                var comment = new Comment();
                comment.Id = ob["data"]["id"].ToString();
                comment.Body = ob["data"]["body"].ToString();
                comment.Author = ob["data"]["author"].ToString();
                comment.Score = ob["data"]["score"].Value<int>();
                comment.Replies = ParseComments(ob["data"]["replies"] as JObject);
                comments.Add(comment);
            }

            return comments;
        }

        public async Task AuthenticateAsync()
        {
            var clientId = "MTcpOsELm-U8sw";
            var authenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri("https://www.reddit.com/api/v1/authorize?client_id=" + clientId + "&response_type=code&state=RANDOM_STRING2&redirect_uri=http://localhost&duration=permanent&scope=identity,edit,flair,history,modconfig,modflair,modlog,modposts,modwiki,mysubreddits,privatemessages,read,report,save,submit,subscribe,vote,wikiedit,wikiread"), new Uri("http://localhost"));
            if(authenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                WwwFormUrlDecoder urlDecoder = new WwwFormUrlDecoder(new Uri(authenticationResult.ResponseData).Query);

                var codeString = urlDecoder.GetFirstValueByName("code");

                var postClient = new HttpClient();
                postClient.DefaultRequestHeaders.Authorization = CreateBasicHeader(clientId, "");

                HttpContent httpContent = new StringContent("grant_type=authorization_code&code=" + codeString + "&redirect_uri=http://localhost");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var responseMessage = await postClient.PostAsync(new Uri("https://ssl.reddit.com/api/v1/access_token"), httpContent);

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(responseContent);

                accessToken = jsonResponse["access_token"].ToString();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }
        }

        private AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            password = SampleHashMsg("MD5", password);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        private String SampleHashMsg(String strAlgName, String strMsg)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(strMsg, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Demonstrate how to retrieve the name of the hashing algorithm.
            String strAlgNameUsed = objAlgProv.AlgorithmName;

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            // Convert the hash to a string (for display).
            String strHashBase64 = CryptographicBuffer.EncodeToHexString(buffHash);

            // Return the encoded string
            return strHashBase64;
        }
    }
}