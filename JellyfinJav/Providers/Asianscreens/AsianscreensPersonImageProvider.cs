#pragma warning disable SA1600

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

    public class AsianscreensPersonImageProvider : IRemoteImageProvider
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Api.AsianscreensClient Client = new Api.AsianscreensClient();

        public string Name => "Asianscreens";

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

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item) => item is Person;
    }
}