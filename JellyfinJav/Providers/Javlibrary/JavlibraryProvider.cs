using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using System.Web;

namespace JellyfinJav.Providers.JavlibraryProvider
{
    public class JavlibraryProvider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        private readonly IHttpClient httpClient;

        public string Name => "Javlibrary";

        public JavlibraryProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancelToken)
        {
            var client = new Javlibrary.Client();

            Javlibrary.Video? result = null;
            if (info.ProviderIds.ContainsKey("Javlibrary"))
                result = await client.LoadVideo(info.ProviderIds["Javlibrary"]);
            else
                result = await client.SearchFirst(info.Name);

            if (!result.HasValue)
                return new MetadataResult<Movie>();

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = info.Name,
                    Name = result.Value.Title,
                    ProviderIds = new Dictionary<string, string> { { "Javlibrary", result.Value.Id } },
                    Studios = new[] { result.Value.Studio }.OfType<string>().ToArray(),
                    Genres = result.Value.Genres.ToArray()
                },
                People = (from actress in result.Value.Actresses
                          select new PersonInfo
                          {
                              Name = actress,
                              Type = "JAV Actress"
                          }).ToList(),
                HasMetadata = true
            };
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            var client = new Javlibrary.Client();
            return from video in await client.Search(info.Name)
                   select new RemoteSearchResult
                   {
                       Name = video.code,
                       ProviderIds = new Dictionary<string, string>
                       {
                           // Functionality should be instead moved into the javlibrary lib
                           { "Javlibrary", HttpUtility.ParseQueryString(video.url.Query).Get("v") }
                       }
                   };
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancelToken
            });
        }
    }
}