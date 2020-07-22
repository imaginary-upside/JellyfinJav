using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.JavlibraryProvider
{
    public class JavlibraryExternalId : IExternalId
    {
        public string ProviderName => "Javlibrary";
        public string Key => "Javlibrary";
        public string UrlFormatString => "https://www.javlibrary.com/en/?v={0}";
        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}