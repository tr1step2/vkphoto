using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VK.WindowsPhone.SDK.API.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace vkphoto
{
    public sealed partial class SeparatedAlbum : UserControl
    {
        public SeparatedAlbum()
        {
            this.InitializeComponent();
        }

        private VKAlbum mAlbum;
        private VKList<VKPhoto> mPhotos;
        private MainPage mMainPage;

        public SeparatedAlbum(VKAlbum album, VKList<VKPhoto> photos, MainPage mainPage)
        {
            this.InitializeComponent();

            mAlbum = album;
            mPhotos = photos;
            mMainPage = mainPage;

            this.RootGrid.Tapped += new TappedEventHandler(onAlbumClick);

            foreach (var p in photos.items)
                addImage(p, mainPage.Frame.ActualWidth - 20);

            title = album.title;
            photos_count = album.size.ToString() + " photos";
        }

        public void addImage(VKPhoto photo, double w)
        {
            ContentPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri(photo.photo_130)),
                                                    Width = w / 3,
                                                    Height = 120,
                                                    Stretch = Stretch.UniformToFill});        
        }

        public void onAlbumClick(object sender, TappedRoutedEventArgs e)
        {
            mMainPage.navigateToAlbumPage(mAlbum);
        }

        public string title { set { Title.Text = value; } }
        public string photos_count { set { PhotosCount.Text = value; } }

    }
}
