namespace JellyfinJav.Providers.JavlibraryProvider
{
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    /// <summary>External ID for a Javlibrary video.</summary>
    public class JavlibraryExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "Javlibrary";

        /// <inheritdoc />
        public string Key => "Javlibrary";

        /// <inheritdoc />
        public string UrlFormatString => "https://www.javlibrary.com/en/?v={0}";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}