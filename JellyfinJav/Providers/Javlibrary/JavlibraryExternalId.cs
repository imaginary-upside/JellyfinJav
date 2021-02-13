#pragma warning disable SA1600, CS1591

namespace JellyfinJav.Providers.JavlibraryProvider
{
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

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