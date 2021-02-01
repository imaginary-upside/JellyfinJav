using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.R18Provider
{
    public class R18ImageProvider : IRemoteImageProvider, IHasOrder
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public string Name => "R18";
        public int Order => 99;
        private readonly IApplicationPaths applicationPaths;

        public R18ImageProvider(IApplicationPaths applicationPaths)
        {
            this.applicationPaths = applicationPaths;
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("R18");
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(new RemoteImageInfo[] { });
            }

            var primaryImage = String.Format("https://pics.r18.com/digital/video/{0}/{0}pl.jpg", id);

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = primaryImage
                }
            });
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