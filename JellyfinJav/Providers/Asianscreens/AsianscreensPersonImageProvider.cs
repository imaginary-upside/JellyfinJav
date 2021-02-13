namespace JellyfinJav.Providers.AsianscreensProvider
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    /// <summary>The provider for Asianscreens actress headshots.</summary>
    public class AsianscreensPersonImageProvider : IRemoteImageProvider
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Api.AsianscreensClient Client = new Api.AsianscreensClient();

        /// <inheritdoc />
        public string Name => "Asianscreens";

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Asianscreens");
            if (string.IsNullOrEmpty(id))
            {
                return Array.Empty<RemoteImageInfo>();
            }

            var actress = await Client.LoadActress(id).ConfigureAwait(false);
            if (!actress.HasValue || actress.Value.Cover == null)
            {
                return Array.Empty<RemoteImageInfo>();
            }

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = this.Name,
                    Type = ImageType.Primary,
                    Url = actress.Value.Cover,
                },
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        /// <inheritdoc />
        public bool Supports(BaseItem item) => item is Person;
    }
}