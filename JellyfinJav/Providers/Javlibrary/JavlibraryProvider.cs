#pragma warning disable SA1600, CS1591

namespace JellyfinJav.Providers.JavlibraryProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Entities.Movies;
    using MediaBrowser.Controller.Library;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Entities;
    using MediaBrowser.Model.Providers;
    using Microsoft.Extensions.Logging;

    public class JavlibraryProvider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly ILibraryManager libraryManager;
        private readonly ILogger<JavlibraryProvider> logger;

        public JavlibraryProvider(
            ILibraryManager libraryManager,
            ILogger<JavlibraryProvider> logger)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
        }

        public string Name => "Javlibrary";

        public int Order => 100;

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancelToken)
        {
            var originalTitle = Utility.GetVideoOriginalTitle(info, this.libraryManager);

            this.logger.LogInformation("[JellyfinJav] Javlibrary - Scanning: " + originalTitle);

            Api.Video? result;
            if (info.ProviderIds.ContainsKey("Javlibrary"))
            {
                result = await JavlibraryClient.LoadVideo(info.ProviderIds["Javlibrary"]).ConfigureAwait(false);
            }
            else
            {
                var code = Utility.ExtractCodeFromFilename(originalTitle);
                if (code is null)
                {
                    return new MetadataResult<Movie>();
                }

                result = await JavlibraryClient.SearchFirst(code).ConfigureAwait(false);
            }

            if (!result.HasValue)
            {
                return new MetadataResult<Movie>();
            }

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = originalTitle,
                    Name = Utility.CreateVideoDisplayName(result.Value),
                    ProviderIds = new Dictionary<string, string> { { "Javlibrary", result.Value.Id } },
                    Studios = new[] { result.Value.Studio },
                    Genres = result.Value.Genres.ToArray(),
                },
                People = CreateActressList(result.Value),
                HasMetadata = true,
            };
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            return from video in await JavlibraryClient.Search(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = video.Code,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Javlibrary", video.Id },
                       },
                   };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        private static string NormalizeActressName(string name)
        {
            if (Plugin.Instance?.Configuration.ActressNameOrder == ActressNameOrder.FirstLast)
            {
                return string.Join(" ", name.Split(' ').Reverse());
            }

            return name;
        }

        private static List<PersonInfo> CreateActressList(Api.Video video)
        {
            if (Plugin.Instance?.Configuration.EnableActresses == false)
            {
                return new List<PersonInfo>();
            }

            return (from actress in video.Actresses
                    select new PersonInfo
                    {
                        Name = NormalizeActressName(actress),
                        Type = PersonType.Actor,
                    }).ToList();
        }
    }
}