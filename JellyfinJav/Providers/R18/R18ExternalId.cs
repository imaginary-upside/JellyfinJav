namespace JellyfinJav.Providers.R18Provider
{
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    /// <summary>External ID for an R18 video.</summary>
    public class R18ExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "R18";

        /// <inheritdoc />
        public string Key => "R18";

        /// <inheritdoc />
        public string UrlFormatString => "https://www.r18.com/videos/vod/movies/detail/-/id={0}/";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}