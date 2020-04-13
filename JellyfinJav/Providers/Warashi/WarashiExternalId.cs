using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.WarashiProvider
{
    public class WarashiExternalId : IExternalId
    {
        public string Name => "Warashi";
        public string Key => "Warashi";
        public string UrlFormatString => "unsupported";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}