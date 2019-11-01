using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.JavlibraryProvider
{
    public class JavlibraryExternalId : IExternalId
    {
        public string Name => "Javlibrary";
        public string Key => "Javlibrary";
        public string UrlFormatString => "http://www.javlibrary.com/en/?v={0}";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}