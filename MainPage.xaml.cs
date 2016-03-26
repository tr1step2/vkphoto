using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;
using VK.WindowsPhone.SDK.Util;
using VK.WindowsPhone.SDK_XAML.Pages;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace vkphoto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Mutex mutex = new Mutex();
        private List<string> mScope = new List<string>{ VKScope.PHOTOS };

        private MainController mMainController;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            mMainController = new MainController(this);

            VKSDK.Initialize("4947267");

            VKSDK.AccessTokenReceived += (s, args) =>
            {
                UpdateUIState();
            };

            VKSDK.AccessDenied += (s, e) =>
            {
                UpdateUIState();
            };

            VKSDK.AccessTokenAccepted += (s, e) =>
            {
                UpdateUIState();
            };

            VKSDK.AccessTokenExpired += (s, e) =>
            {
                UpdateUIState();
            };

            VKSDK.AccessTokenReceived += (s, e) =>
            {
                UpdateUIState();
            };

            VKSDK.AccessTokenRenewed += (s, e) =>
            {
                UpdateUIState();
            };

            VKSDK.CaptchaRequest = (captchaUserRequest, action) =>
            {
                new VKCaptchaRequestUserControl().ShowCaptchaRequest(captchaUserRequest, action);
            };

            VKSDK.WakeUpSession();

            VKSDK.Authorize(mScope, false, true, LoginType.WebView);
        }

        private void UpdateUIState()
        {
            if (VKSDK.IsLoggedIn)
                mMainController.onAuthSuccess();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void showAll()
        {
            LayoutRoot.Visibility = Visibility.Visible;
            AppBar.Visibility = Visibility.Visible;
        }

        public void setUserInfo(VKUser user)
        {
            UserNameTextBlock.Text = user.first_name.ToUpper() + " " + user.last_name.ToUpper();

            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(user.photo_200);
            AvatarImageBrush.ImageSource = bitmapImage;
        }

        private int getBestQuality(List<VKSize> list)
        {
            int stored = 0;
            for (int i = 0; i < list.Count(); ++i)
            {
                if (list[i].height >= list[stored].height ||
                    list[i].width >= list[stored].width)
                    stored = i;
            }

            return stored;
        }

        List<int> albumsIndecies = new List<int>();
        List<VKAlbum> mAlbums = new List<VKAlbum>();

        public void addAlbum(VKAlbum album)
        {
            if (albumsIndecies.Contains(album.id))
                return;

            mAlbums.Add(album);
            ContentPanel.Children.Add(new Album(album, this));
            albumsIndecies.Add(album.id);
        }

        public void addSystemAlbum(VKAlbum album, VKList<VKPhoto> photos)
        {
            if (albumsIndecies.Contains(album.id))
                return;

            mAlbums.Add(album);
            ContentPanel.Children.Add(new SeparatedAlbum(album, photos, this));
            albumsIndecies.Add(album.id);
        }

        public void navigateToAlbumPage(VKAlbum album)
        {
            Frame.Navigate(typeof(AlbumPage), album);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            onDownloadStart();

            ImageDownloader downloader = new ImageDownloader();
            foreach(var album in mAlbums)
            {
                downloader.downloadAlbum(album, ()=>{ }, onDownloadFinished, onError);
            }
        }

        int mCurrentAlbum = 0;
        bool inProcess = false;
        void onDownloadStart()
        {
            mutex.WaitOne();

            if (!inProcess)
            {
                inProcess = true;

                VKExecute.ExecuteOnUIThread(() => 
                {
                    this.ContentGrid.Visibility = Visibility.Collapsed;
                    this.DownloadButton.IsEnabled = false;
                    this.ProgRing.IsActive = true;
                });
                
            }

            mutex.ReleaseMutex();
        }

        void onDownloadFinished()
        {
            mutex.WaitOne();

            mCurrentAlbum += 1;
            if (mCurrentAlbum >= mAlbums.Count - 1)
            { 
                mCurrentAlbum = 0;
                inProcess = false;

                VKExecute.ExecuteOnUIThread(()=>
                {
                    this.ContentGrid.Visibility = Visibility.Visible;
                    this.DownloadButton.IsEnabled = true;
                    this.ProgRing.IsActive = false;
                });
                
            }

            mutex.ReleaseMutex();
        }

        void onError(Exception e)
        {

        }
    }
}
