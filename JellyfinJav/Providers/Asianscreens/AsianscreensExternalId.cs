using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.AsianscreensProvider
{
    public class AsianscreensExternalId : IExternalId
    {
        public string Name => "Asianscreens";
        public string Key => "Asianscreens";
        public string UrlFormatString => "https://www.asianscreens.com/{0}.asp";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}