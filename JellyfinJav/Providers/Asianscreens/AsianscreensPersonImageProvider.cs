using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using System;

namespace JellyfinJav.Providers.Asianscreens
{
    public class AsianscreensPersonImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;

        public string Name => "Asianscreens";

        public AsianscreensPersonImageProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var result = new List<RemoteImageInfo>();

            var client = new AsianscreensApi();

            var id = item.GetProviderId("Asianscreens");
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(result);
            }

            var idEnd = id[id.Length - 1];
            var picEnd = "";
            if (idEnd == '2')
            {
                picEnd = "";
            }
            else
            {
                picEnd = (Char.GetNumericValue(idEnd) - 1).ToString();
            }

            var coverImage = string.Format(
                "https://www.asianscreens.com/products/400000/portraits/{0}{1}.jpg",
                id.TrimEnd(idEnd), picEnd
            );

            result.Add(new RemoteImageInfo
            {
                ProviderName = Name,
                Type = ImageType.Primary,
                Url = coverImage
            });

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(result);
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancelToken
            });
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item)
        {
            return item is Person;
        }
    }
}