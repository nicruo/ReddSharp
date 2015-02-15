using Nicruo.ReddSharp.Demo.Common;
using Nicruo.ReddSharp.Domain;
using Nicruo.ReddSharp.WindowsStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nicruo.ReddSharp.Demo
{
    public sealed partial class CommentsPage : Page, INotifyPropertyChanged
    {

        private NavigationHelper _navigationHelper;        
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        private Post _post;
        public Post Post
        {
            get { return _post; }
            set { SetProperty(ref _post, value); }
        }

        private IList<Comment> _comments;
        public IList<Comment> Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }



        public CommentsPage()
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
            string id = (string)e.Parameter;

            RedditService redditService = new RedditService();
            PostComments postComments = await redditService.GetPostCommentsAsync(id);
            Post = postComments.Post;
            Comments = postComments.Comments;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
