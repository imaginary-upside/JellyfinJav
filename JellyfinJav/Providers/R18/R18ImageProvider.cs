using System;
using System.IO;
using SkiaSharp;
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

            // probably should be downloading the full size image, and then cropping the front cover
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
            await CropThumb(httpResponse, Path.GetFileName(url));
            return httpResponse;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item) => item is Movie;

        private async Task CropThumb(HttpResponseMessage httpResponse, string filename)
        {
            using (var imageStream = await httpResponse.Content.ReadAsStreamAsync())
            {
                using (var imageBitmap = SKBitmap.Decode(imageStream))
                {
                    SKBitmap subset = new SKBitmap();
                    imageBitmap.ExtractSubset(subset, SKRectI.Create(421, 0, 379, 538));

                    // I think there will be a memory leak if I use MemoryStore.
                    Directory.CreateDirectory(Path.Combine(applicationPaths.TempDirectory, "JellyfinJav"));
                    var finalStream = File.Open(Path.Combine(applicationPaths.TempDirectory, "JellyfinJav", filename), FileMode.OpenOrCreate);
                    subset.Encode(finalStream, SKEncodedImageFormat.Jpeg, 100);
                    finalStream.Seek(0, 0);

                    var newContent = new StreamContent(finalStream);
                    newContent.Headers.ContentType = httpResponse.Content.Headers.ContentType;
                    httpResponse.Content = newContent;
                }
            }
        }
    }
}