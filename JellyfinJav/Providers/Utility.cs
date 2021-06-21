namespace JellyfinJav.Providers
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Library;
    using MediaBrowser.Controller.Providers;
    using SkiaSharp;

    /// <summary>A general utility class for random functions.</summary>
    public static class Utility
    {
        /// <summary>
        /// When setting the video title in a Provider, we lose the JAV code details in MovieInfo.
        /// So this is used to retrieve the JAV code to then be able to search using a different Provider.
        /// </summary>
        /// <param name="info">The video's info.</param>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager" />.</param>
        /// <returns>The video's original title.</returns>
        public static string GetVideoOriginalTitle(MovieInfo info, ILibraryManager libraryManager)
        {
            var searchQuery = new InternalItemsQuery
            {
                Name = info.Name,
            };
            var result = libraryManager.GetItemList(searchQuery).FirstOrDefault();

            if (result is null)
            {
                return info.Name;
            }

            return result.OriginalTitle ?? result.Name;
        }

        /// <summary>Extracts the jav code from a video's filename.</summary>
        /// <param name="filename">The video's filename.</param>
        /// <returns>The video's jav code.</returns>
        public static string? ExtractCodeFromFilename(string filename)
        {
            var rx = new Regex(@"[\w\d]+-?\d+");
            var value = rx.Match(filename)?.Value.ToUpper();

            if (value is null)
            {
                return null;
            }

            if (value.Contains("-"))
            {
                return value;
            }
            else
            {
                rx = new Regex(@"([\w\d]+?)(\d+)");
                var groups = rx.Match(value).Groups;
                return groups[1] + "-" + groups[2];
            }
        }

        /// <summary>Creates a video's display name according to the plugin's selected configuration.</summary>
        /// <param name="video">The video.</param>
        /// <returns>The video's created display name.</returns>
        public static string CreateVideoDisplayName(Api.Video video)
        {
            return Plugin.Instance?.Configuration.VideoDisplayName switch
            {
                VideoDisplayName.CodeTitle => video.Code + " " + video.Title,
                VideoDisplayName.Title => video.Title,
                _ => throw new System.Exception("Impossible to reach.")
            };
        }

        /// <summary>Crops a full size dvd cover into just the front cover image.</summary>
        /// <param name="httpResponse">The full size dvd cover's http response.</param>
        /// <returns>An empty task when the job is done.</returns>
        public static async Task CropThumb(HttpResponseMessage httpResponse)
        {
            using var imageStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var imageBitmap = SKBitmap.Decode(imageStream);

            SKBitmap subset = new SKBitmap();
            imageBitmap.ExtractSubset(subset, SKRectI.Create(421, 0, 379, 538));

            // I think there will be a memory leak if I use MemoryStore.
            var finalStream = File.Open(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg"), FileMode.OpenOrCreate);
            subset.Encode(finalStream, SKEncodedImageFormat.Jpeg, 100);
            finalStream.Seek(0, 0);

            var newContent = new StreamContent(finalStream);
            newContent.Headers.ContentType = httpResponse.Content.Headers.ContentType;
            httpResponse.Content = newContent;
        }
    }
}