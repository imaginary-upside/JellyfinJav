using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using System;

namespace JellyfinJav.Providers.AsianscreensProvider
{
    public class AsianscreensPersonImageProvider : IRemoteImageProvider
    {
        private readonly static HttpClient httpClient = new HttpClient();
        private readonly static Api.AsianscreensClient client = new Api.AsianscreensClient();

        public string Name => "Asianscreens";

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Asianscreens");
            if (string.IsNullOrEmpty(id))
                return Array.Empty<RemoteImageInfo>();

            var actress = await client.LoadActress(id).ConfigureAwait(false);
            if (!actress.HasValue || actress.Value.Cover == null)
                return Array.Empty<RemoteImageInfo>();

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = actress.Value.Cover
                }
            };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await httpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item) => item is Person;
    }
}