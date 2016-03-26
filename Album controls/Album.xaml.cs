using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VK.WindowsPhone.SDK.API.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Album : UserControl
    {
        public Album()
        {
            this.InitializeComponent();
        }

        MainPage mMainPage;
        VKAlbum mAlbum;
        public Album(VKAlbum album, MainPage mainPage)
        {
            this.InitializeComponent();

            mMainPage = mainPage;
            mAlbum = album;

            //set up onClick handler
            this.MainGrid.Tapped += new TappedEventHandler(onAlbumClick);

            this.thubm_src = getSource(album.sizes, "q");
            this.title = album.title;
            this.photos_count = album.size.ToString() + " photos";
        }

        public void onAlbumClick(object sender, TappedRoutedEventArgs e)
        {
            mMainPage.navigateToAlbumPage(mAlbum);
        }

        string getSource(List<VKSize> sizes, string type)
        {
            foreach(var size in sizes)
            {
                if (size.type == "q")
                    return size.src;
            }

            return sizes[sizes.Count - 1].src;
        }

        public string thubm_src { set { ThumbSrc.Source = new BitmapImage(new Uri(value)); } }
        public string title { set { Title.Text = value; } }
        public string photos_count { set { PhotosCount.Text = value; } }
    }
}
