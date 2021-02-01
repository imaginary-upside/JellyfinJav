using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
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

            var client = new Api.JavlibraryClient();
            var video = await client.LoadVideo(id);

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = video.BoxArt
                }
            };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            var httpResponse = await httpClient.GetAsync(url).ConfigureAwait(false);
            await Utility.CropThumb(httpResponse);
            return httpResponse;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item) => item is Movie;
    }
}