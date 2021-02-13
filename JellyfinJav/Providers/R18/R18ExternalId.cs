#pragma warning disable SA1600, CS1591

namespace JellyfinJav.Providers.R18Provider
{
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    public class R18ExternalId : IExternalId
    {
        public string ProviderName => "R18";

        public string Key => "R18";

        public string UrlFormatString => "https://www.r18.com/videos/vod/movies/detail/-/id={0}/";

        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}