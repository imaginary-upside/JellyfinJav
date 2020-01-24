using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using R18;

namespace JellyfinJav.Providers.R18Provider
{
    public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private readonly IServerConfigurationManager configManager;
        private readonly IHttpClient httpClient;
        private readonly ILibraryManager libraryManager;
        private readonly ILogger logger;
        private readonly R18.Client client = new R18.Client();

        public string Name => "R18";
        public int Order => 10;

        public R18Provider(IServerConfigurationManager configManager,
                           IHttpClient httpClient,
                           ILibraryManager libraryManager,
                           ILogger logger)
        {
            this.configManager = configManager;
            this.httpClient = httpClient;
            this.libraryManager = libraryManager;
            this.logger = logger;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                             CancellationToken cancelToken)
        {
            var originalTitle = Utility.GetVideoOriginalTitle(info, libraryManager);
            info.Name = originalTitle;

            logger.LogInformation("[JellyfinJav] R18 - Scanning: " + originalTitle);

            R18.Video? video = null;
            if (info.ProviderIds.ContainsKey("R18"))
                video = await client.LoadVideo(info.ProviderIds["R18"]);
            else
                video = await client.SearchFirst(Utility.ExtractCodeFromFilename(originalTitle));

            if (!video.HasValue)
                return new MetadataResult<Movie>();

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = info.Name,
                    Name = video.Value.Title,
                    PremiereDate = video.Value.ReleaseDate,
                    ProviderIds = new Dictionary<string, string> { { "R18", video.Value.Id } },
                    Studios = new[] { video.Value.Studio }.OfType<string>().ToArray(),
                    Genres = video.Value.Genres.OfType<string>().ToArray()
                },
                People = (from actress in video.Value.Actresses
                          select new PersonInfo
                          {
                              Name = NormalizeActressName(actress),
                              Type = "JAV Actress"
                          }).ToList(),
                HasMetadata = true
            };
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            var javCode = Utility.ExtractCodeFromFilename(info.Name);
            if (string.IsNullOrEmpty(javCode))
                return new RemoteSearchResult[] { };

            return from video in await client.Search(javCode)
                   select new RemoteSearchResult
                   {
                       Name = video.code,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "R18", video.id }
                       },
                       ImageUrl = video.cover.ToString()
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


        private static string NormalizeActressName(string name)
        {
            if (Plugin.Instance.Configuration.actressNameOrder == ActressNameOrder.LastFirst)
                return string.Join(" ", name.Split(' ').Reverse());
            return name;
        }
    }
}