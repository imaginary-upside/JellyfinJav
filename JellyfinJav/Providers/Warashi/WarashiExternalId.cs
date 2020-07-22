using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.WarashiProvider
{
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