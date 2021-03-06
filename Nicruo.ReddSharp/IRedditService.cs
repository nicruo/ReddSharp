﻿using Nicruo.ReddSharp.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nicruo.ReddSharp
{
    public interface IRedditService
    {
        Task<IList<string>> GetSubredditsAsync();
        Task<Subreddit> GetSubredditAsync(string subredditName);
        Task<SubredditAbout> GetSubredditAboutAsync(string subredditName);
        Task<IList<string>> SearchForSubredditsAsync(string query);
        Task<PostComments> GetPostCommentsAsync(string id);
    }
}