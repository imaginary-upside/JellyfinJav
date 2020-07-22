using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.AsianscreensProvider
{
    public class AsianscreensExternalId : IExternalId
    {
        public string ProviderName => "Asianscreens";
        public string Key => "Asianscreens";
        public string UrlFormatString => "https://www.asianscreens.com/{0}.asp";
        public ExternalIdMediaType? Type => ExternalIdMediaType.Person;

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}