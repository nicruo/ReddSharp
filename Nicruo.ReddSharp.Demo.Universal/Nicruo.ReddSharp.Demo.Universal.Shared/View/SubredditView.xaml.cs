using Nicruo.ReddSharp.Demo.Universal.Common;
using Nicruo.ReddSharp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nicruo.ReddSharp.Demo.Universal.View
{
    public sealed partial class SubredditView : Page, INotifyPropertyChanged
    {

        private NavigationHelper _navigationHelper;        
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IList<Post> _posts;
        public IList<Post> Posts
        {
            get { return _posts; }
            set { SetProperty(ref _posts, value); }
        }

        private SubredditAbout _about;
        public SubredditAbout About
        {
            get { return _about; }
            set { SetProperty(ref _about, value); }
        }



        public SubredditView()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;
            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
            string subredditName = (string)e.Parameter;
            Title = subredditName;

            RedditService redditService = new RedditService();
            Subreddit subreddit = await redditService.GetSubredditAsync(subredditName);
            Posts = subreddit.Posts;
            About = await redditService.GetSubredditAboutAsync(subredditName);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(PostView), (e.ClickedItem as Post).Id);
        }

        private void Comments_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CommentsView), (sender as Button).Tag);
        }
    }
}
