using vkphoto.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API.Model;
using Windows.UI.Xaml.Media.Imaging;
using VK.WindowsPhone.SDK.Util;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace vkphoto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private VKAlbum mAlbum;

        public AlbumPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            mAlbum = (VKAlbum)e.NavigationParameter;
            updateUI();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void updateUI()
        {
            HeadImage.Source = new BitmapImage(new Uri(getSource(mAlbum.sizes, "q")));

            if (mAlbum.description.Length > 0)
            {
                DescriptionTextBlock.Text = mAlbum.description;
                DescriptionTextBlock.Visibility = Visibility.Visible;
            }
            
        }

        private string getSource(List<VKSize> sizes, string type)
        {
            foreach (var size in sizes)
            {
                if (size.type == "q")
                    return size.src;
            }

            return sizes[sizes.Count - 1].src;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            ImageDownloader downloader = new ImageDownloader();

            onDownloadStart();
            downloader.downloadAlbum(mAlbum, () => { }, onDownLoadFinished, onError);
        }

        void onDownloadStart()
        {
            VKExecute.ExecuteOnUIThread( () =>
            {
                DownloadButton.IsEnabled = false;
                this.ProgRing.IsEnabled = true;
                this.ProgRing.IsActive = true;
            });
           
        }

        void onDownLoadFinished()
        {
            VKExecute.ExecuteOnUIThread(() =>
            {
                DownloadButton.IsEnabled = true;
                this.ProgRing.IsActive = false;
                this.ProgRing.IsEnabled = false;
            });
            
        }

        void onError(Exception e)
        {
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        
    }
}
