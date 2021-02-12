#pragma warning disable SA1600

namespace JellyfinJav.Providers.JavlibraryProvider
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    public class JavlibraryImageProvider : IRemoteImageProvider, IHasOrder
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public string Name => "Javlibrary";

        public int Order => 100;

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Javlibrary");
            if (string.IsNullOrEmpty(id))
            {
                return Array.Empty<RemoteImageInfo>();
            }

            var video = await JavlibraryClient.LoadVideo(id).ConfigureAwait(false);

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = this.Name,
                    Type = ImageType.Primary,
                    Url = video.BoxArt,
                },
            };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            var httpResponse = await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
            await Utility.CropThumb(httpResponse).ConfigureAwait(false);
            return httpResponse;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item) => item is Movie;
    }
}