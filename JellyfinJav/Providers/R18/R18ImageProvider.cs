namespace JellyfinJav.Providers.R18Provider
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    /// <summary>The provider for R18 video covers.</summary>
    public class R18ImageProvider : IRemoteImageProvider, IHasOrder
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>Initializes a new instance of the <see cref="R18ImageProvider"/> class.</summary>
        public R18ImageProvider()
        {
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        /// <inheritdoc />
        public string Name => "R18";

        /// <inheritdoc />
        public int Order => 99;

        /// <inheritdoc />
        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("R18");
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(Array.Empty<RemoteImageInfo>());
            }

            var primaryImage = string.Format("https://pics.r18.com/digital/video/{0}/{0}pl.jpg", id);

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = this.Name,
                    Type = ImageType.Primary,
                    Url = primaryImage,
                },
            });
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            var httpResponse = await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
            await Utility.CropThumb(httpResponse).ConfigureAwait(false);
            return httpResponse;
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        /// <inheritdoc />
        public bool Supports(BaseItem item) => item is Movie;
    }
}