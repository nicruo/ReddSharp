using Nicruo.ReddSharp.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Nicruo.ReddSharp.Demo.Universal.Common
{
    public sealed partial class CommentControl : UserControl
    {
        public Comment Comment
        {
            get { return (Comment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register("Comment", typeof(Comment), typeof(CommentControl), new PropertyMetadata(null));        

        public CommentControl()
        {
            this.InitializeComponent();
            root.DataContext = this;
        }
    }
}
