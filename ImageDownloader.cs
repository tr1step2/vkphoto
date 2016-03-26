using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API.Model;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using VK.WindowsPhone.SDK.API;
using System.Text.RegularExpressions;
using Windows.System.Threading;

namespace vkphoto
{
    class ImageDownloader
    {
        Dictionary<int, string> systemAlbumsNamesMap = new Dictionary<int, string>() { { -6, "profile" }, { -7, "wall" }, { -15, "saved" } };
        string mInvalidChars = new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()) + ".,!";
        HttpClient mHttpClient = new HttpClient();
        string mFolderRootname = "vkphoto";
        string fileend = ".jpg";

        public void downloadAlbum(VKAlbum album, Action callbackStarted, Action callbackFinished, Action<Exception> callbackError)
        {
            string album_id = album.id < 0 ? systemAlbumsNamesMap[album.id] : album.id.ToString();
            VKRequest.Dispatch<VKList<VKPhoto>>(
                new VKRequestParameters("photos.get", "album_id", album_id, "rev", "1", "photo_sizes", "1"),
                (res) =>
                {
                    if (res.ResultCode == VKResultCode.Succeeded && res.Data.count > 0)
                    {
                        callbackStarted();
                                                
                        for (int i = 0; i < res.Data.count; ++i)
                        {
                            var photo = res.Data.items[i];

                            string folderName = getFolderName(album.title);

                            string src = getBestQualitySource(photo.sizes);

                            try
                            {
                                Task task = Task.Run(() => { downloadImage(mFolderRootname, folderName, i.ToString() + fileend, src); });
                                task.Wait();
                            }

                            catch (Exception e)
                            {
                                callbackError(e);
                            }    
                        }
                    }

                    callbackFinished();
                });
        }

        private string getFolderName(string title)
        {
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(mInvalidChars)));
            return r.Replace(title, " ");
        }

        private async void downloadImage(string folderRootName, string folderDestName, string filename, string src)
        {
            try
            {
                StorageFolder sfolderRoot = await KnownFolders.SavedPictures.CreateFolderAsync(folderRootName, CreationCollisionOption.OpenIfExists);
                StorageFolder sfolderDest = await sfolderRoot.CreateFolderAsync(folderDestName, CreationCollisionOption.OpenIfExists);

                StorageFile sfile = await sfolderDest.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                var buf = await mHttpClient.GetByteArrayAsync(new Uri(src));

                await FileIO.WriteBytesAsync(sfile, buf);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        private string getBestQualitySource(List<VKSize> sizes)
        {
            return getBestSizeByType(sizes);
        }

        Dictionary<string, int> mTypesDict = new Dictionary<string, int>() { {"s", 0 }, {"m", 1 }, { "o", 2 }, { "p", 4 }, { "q", 8 }, { "r", 16 },
            { "x", 32 }, { "y", 64 }, { "z", 128 }, { "w", 256 } };

        private string getBestSizeByType(List<VKSize> sizes)
        {
            var best = sizes[0];
            int bestvalue = 0;
            int newvalue = 0;
            foreach (var size in sizes)
            {
                newvalue = mTypesDict[size.type];
                if (newvalue > bestvalue)
                {
                    bestvalue = newvalue;
                    best = size;
                }
            }

            return best.src;
        }
    }
}
