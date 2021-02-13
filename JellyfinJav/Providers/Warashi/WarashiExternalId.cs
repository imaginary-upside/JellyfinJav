namespace JellyfinJav.Providers.WarashiProvider
{
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    /// <summary>External ID for a Warashi actress.</summary>
    public class WarashiExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "Warashi";

        /// <inheritdoc />
        public string Key => "Warashi";

        /// <inheritdoc />
        public string UrlFormatString => "unsupported";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Person;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}