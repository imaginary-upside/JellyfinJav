using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.Javlibrary {
    public class JavlibraryExternalId : IExternalId {
        public string Name => "Javlibrary";
        public string Key => "Javlibrary";
        public string UrlFormatString => "https://www.javlibrary/en/?v={0}";

        public bool Supports(IHasProviderIds item) {
            return item is Movie;
        }
    }
}