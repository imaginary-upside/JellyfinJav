#pragma warning disable SA1600

namespace JellyfinJav.Providers.WarashiProvider
{
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;

    public class WarashiExternalId : IExternalId
    {
        public string ProviderName => "Warashi";

        public string Key => "Warashi";

        public string UrlFormatString => "unsupported";

        public ExternalIdMediaType? Type => ExternalIdMediaType.Person;

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}