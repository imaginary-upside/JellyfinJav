using System.Linq;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using SkiaSharp;

namespace JellyfinJav.Providers
{
    public class Utility
    {
        // When setting the video title in a Provider, we lose the JAV code details in MovieInfo.
        // So this is used to retrieve the JAV code to then be able to search using a different Provider.
        public static string GetVideoOriginalTitle(MovieInfo info, ILibraryManager libraryManager)
        {
            var searchQuery = new InternalItemsQuery
            {
                Name = info.Name
            };
            var result = libraryManager.GetItemList(searchQuery).FirstOrDefault();

            if (result == null)
                return info.Name;

            return result.OriginalTitle ?? result.Name;
        }

        public static string ExtractCodeFromFilename(string filename)
        {
            var rx = new Regex(@"[\w\d]+-\d+", RegexOptions.Compiled);
            var match = rx.Match(filename);
            return match?.Value.ToUpper();
        }

        public static string CreateVideoDisplayName(Api.Video video)
        {
            switch(Plugin.Instance.Configuration.videoDisplayName)
            {
            case VideoDisplayName.CodeTitle:
                return video.Code + " " + video.Title;
            case VideoDisplayName.Title:
                return video.Title;
            default:
                // This should be impossible to reach.
                return null;
            }
        }

        public static async Task CropThumb(HttpResponseMessage httpResponse)
        {
            using (var imageStream = await httpResponse.Content.ReadAsStreamAsync())
            {
                using (var imageBitmap = SKBitmap.Decode(imageStream))
                {
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
    }
}