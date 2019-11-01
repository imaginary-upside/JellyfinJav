using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.JavlibraryProvider
{
    public class JavlibraryImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;

        public string Name => "Javlibrary";

        public JavlibraryImageProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Javlibrary");
            if (string.IsNullOrEmpty(id))
                return new RemoteImageInfo[] { };

            // probably should be downloading the full size image, and then cropping the front cover
            var client = new Javlibrary.Client();
            var video = await client.LoadVideo(id);

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = video.Cover
                },
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Thumb,
                    Url = video.BoxArt
                }
            };
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
            return new[] { ImageType.Primary, ImageType.Thumb };
        }

        public bool Supports(BaseItem item)
        {
            return item is Movie;
        }
    }
}