using System;
using System.Linq;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using System.Text.RegularExpressions;

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
    }
}