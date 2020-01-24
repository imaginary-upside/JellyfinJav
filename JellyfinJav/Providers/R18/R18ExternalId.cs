using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;

namespace JellyfinJav.Providers.R18Provider
{
    public class R18ExternalId : IExternalId
    {
        public string Name => "R18";
        public string Key => "R18";
        public string UrlFormatString => "https://www.r18.com/videos/vod/movies/detail/-/id={0}/";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}