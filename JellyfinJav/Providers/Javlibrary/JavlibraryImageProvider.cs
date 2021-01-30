using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.JavlibraryProvider
{
    public class JavlibraryImageProvider : IRemoteImageProvider, IHasOrder
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public string Name => "Javlibrary";
        public int Order => 100;

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Javlibrary");
            if (string.IsNullOrEmpty(id))
                return new RemoteImageInfo[] { };

            // probably should be downloading the full size image, and then cropping the front cover
            var client = new Api.JavlibraryClient();
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

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await httpClient.GetAsync(url).ConfigureAwait(false);
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary, ImageType.Thumb };
        }

        public bool Supports(BaseItem item) => item is Movie;
    }
}