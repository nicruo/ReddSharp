﻿using Nicruo.ReddSharp.Demo.Common;
using Nicruo.ReddSharp.WindowsStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nicruo.ReddSharp.Demo
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private NavigationHelper _navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        private IList<string> _subreddits;
        public IList<string> Subreddits
        {
            get { return _subreddits; }
            set { SetProperty(ref _subreddits, value); }
        }

        public string Title { get; set; }


        public MainPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
            RedditService redditService = new RedditService();

            var subreddits = await redditService.GetSubredditsAsync();
            Subreddits = new List<string>(subreddits);

            await redditService.GetPostCommentsAsync("2vvaj6");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
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

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(SubredditPage), e.ClickedItem);
        }
    }
}