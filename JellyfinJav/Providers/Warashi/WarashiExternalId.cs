using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.WarashiProvider
{
    public class WarashiExternalId : IExternalId
    {
        public string Name => "Warashi";
        public string Key => "Warashi";
        public string UrlFormatString => "http://warashi-asian-pornstars.fr/en/s-2-0/anything/asian-female-pornstar/{0}";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}