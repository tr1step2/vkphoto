using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;
using VK.WindowsPhone.SDK.Util;

namespace vkphoto
{
    class MainController
    {
        MainPage mMainPage;

        public MainController(MainPage mainPage)
        {
            mMainPage = mainPage;
        }

        public void onAuthSuccess()
        {
            setHeaders();
            fillAlbums();
            showAll();
        }

        private void setHeaders()
        {
            VKRequest.Dispatch<List<VKUser>>(
               new VKRequestParameters("users.get", "fields", "photo_200"),
               (res) =>
               {
                   if (res.ResultCode == VKResultCode.Succeeded)
                   {
                       VKExecute.ExecuteOnUIThread(() =>
                       {
                           mMainPage.setUserInfo(res.Data[0]);
                       });
                   }
               });
        }

        private void fillSystemAlbum(VKAlbum album)
        {
            string album_id = systemAlbumsNamesMap[album.id];

            VKRequest.Dispatch<VKList<VKPhoto>>(
                new VKRequestParameters("photos.get", "album_id", album_id, "rev", "1", "count", "3"),
                (res) =>
                {
                    if (res.ResultCode == VKResultCode.Succeeded && res.Data.count > 0)
                    {
                        VKExecute.ExecuteOnUIThread(() =>
                        {
                            mMainPage.addSystemAlbum(album, res.Data);
                        });
                    }
                });
        }

        Dictionary<int, VKAlbum> systemAlbums = new Dictionary<int, VKAlbum>();
        Dictionary<int, string> systemAlbumsNamesMap = new Dictionary<int, string>() { { -6, "profile" }, { -7, "wall" }, { -15, "saved" } };
        private void fillAlbums()
        {
            VKRequest.Dispatch<VKList<VKAlbum>>(
                new VKRequestParameters("photos.getAlbums", "need_covers", "1", "need_system", "1", "photo_sizes", "1"),  
                (res) => 
                {
                    if (res.ResultCode == VKResultCode.Succeeded && res.Data.count > 0)
                    {
                        foreach (var album in res.Data.items)
                        {
                            if (album.id < 0)
                                fillSystemAlbum(album);
                            else
                            {
                                VKExecute.ExecuteOnUIThread(() =>
                                {
                                    mMainPage.addAlbum(album);
                                });
                            }
                        }
                    }
                });
        }

        private void showAll()
        {
            mMainPage.showAll();
        }
    }
}
