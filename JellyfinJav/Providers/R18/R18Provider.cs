using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace JellyfinJav.Providers.R18Provider
{
    public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly ILibraryManager libraryManager;
        private readonly ILogger<R18Provider> logger;
        private readonly Api.R18Client client = new Api.R18Client();

        public string Name => "R18";
        public int Order => 99;

        public R18Provider(ILibraryManager libraryManager,
                           ILogger<R18Provider> logger)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                             CancellationToken cancelToken)
        {
            var originalTitle = Utility.GetVideoOriginalTitle(info, libraryManager);
            info.Name = originalTitle;

            logger.LogInformation("[JellyfinJav] R18 - Scanning: " + originalTitle);

            Api.Video? video;
            if (info.ProviderIds.ContainsKey("R18"))
                video = await client.LoadVideo(info.ProviderIds["R18"]).ConfigureAwait(false);
            else
                video = await client.SearchFirst(Utility.ExtractCodeFromFilename(originalTitle)).ConfigureAwait(false);

            if (!video.HasValue)
                return new MetadataResult<Movie>();

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = info.Name,
                    Name = Utility.CreateVideoDisplayName(video.Value),
                    PremiereDate = video.Value.ReleaseDate,
                    ProviderIds = new Dictionary<string, string> { { "R18", video.Value.Id } },
                    Studios = new[] { video.Value.Studio },
                    Genres = video.Value.Genres.ToArray()
                },
                People = CreateActressList(video.Value),
                HasMetadata = true
            };
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            var javCode = Utility.ExtractCodeFromFilename(info.Name);
            if (string.IsNullOrEmpty(javCode))
                return Array.Empty<RemoteSearchResult>();

            return from video in await client.Search(javCode).ConfigureAwait(false)
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

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await httpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        private static string NormalizeActressName(string name)
        {
            if (Plugin.Instance.Configuration.ActressNameOrder == ActressNameOrder.LastFirst)
                return string.Join(" ", name.Split(' ').Reverse());
            return name;
        }

        private static List<PersonInfo> CreateActressList(Api.Video video)
        {
            if (!Plugin.Instance.Configuration.EnableActresses)
                return new List<PersonInfo>();

            return (from actress in video.Actresses
                    select new PersonInfo
                    {
                        Name = NormalizeActressName(actress),
                        Type = PersonType.Actor
                    }).ToList();
        }
    }
}