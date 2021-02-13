namespace JellyfinJav.Providers.R18Provider
{
    using System;
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

    /// <summary>The provider for R18 videos.</summary>
    public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly ILibraryManager libraryManager;
        private readonly ILogger<R18Provider> logger;

        /// <summary>Initializes a new instance of the <see cref="R18Provider"/> class.</summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager" />.</param>
        /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
        public R18Provider(ILibraryManager libraryManager, ILogger<R18Provider> logger)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
        }

        /// <inheritdoc />
        public string Name => "R18";

        /// <inheritdoc />
        public int Order => 99;

        /// <inheritdoc />
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancelToken)
        {
            var originalTitle = Utility.GetVideoOriginalTitle(info, this.libraryManager);
            info.Name = originalTitle;

            this.logger.LogInformation("[JellyfinJav] R18 - Scanning: " + originalTitle);

            Api.Video? video;
            if (info.ProviderIds.ContainsKey("R18"))
            {
                video = await R18Client.LoadVideo(info.ProviderIds["R18"]).ConfigureAwait(false);
            }
            else
            {
                var code = Utility.ExtractCodeFromFilename(originalTitle);
                if (code is null)
                {
                    return new MetadataResult<Movie>();
                }

                video = await R18Client.SearchFirst(code).ConfigureAwait(false);
            }

            if (!video.HasValue)
            {
                return new MetadataResult<Movie>();
            }

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = info.Name,
                    Name = Utility.CreateVideoDisplayName(video.Value),
                    PremiereDate = video.Value.ReleaseDate,
                    ProviderIds = new Dictionary<string, string> { { "R18", video.Value.Id } },
                    Studios = new[] { video.Value.Studio },
                    Genres = video.Value.Genres.ToArray(),
                },
                People = CreateActressList(video.Value),
                HasMetadata = true,
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            var javCode = Utility.ExtractCodeFromFilename(info.Name);
            if (string.IsNullOrEmpty(javCode))
            {
                return Array.Empty<RemoteSearchResult>();
            }

            return from video in await R18Client.Search(javCode).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = video.Code,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "R18", video.Id },
                       },
                       ImageUrl = video.Cover?.ToString(),
                   };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        private static string NormalizeActressName(string name)
        {
            if (Plugin.Instance?.Configuration.ActressNameOrder == ActressNameOrder.LastFirst)
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