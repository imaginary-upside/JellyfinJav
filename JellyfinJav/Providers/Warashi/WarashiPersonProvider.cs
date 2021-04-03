namespace JellyfinJav.Providers.AsianscreensProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Providers;
    using Microsoft.Extensions.Logging;

    /// <summary>The provider for Warashi actresses.</summary>
    public class WarashiPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly ILogger<WarashiPersonProvider> logger;

        /// <summary>Initializes a new instance of the <see cref="WarashiPersonProvider"/> class.</summary>
        /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
        public WarashiPersonProvider(ILogger<WarashiPersonProvider> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string Name => "Warashi";

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            return from actress in await WarashiClient.Search(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = actress.Name,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Warashi", actress.Id },
                       },
                       ImageUrl = actress.Cover?.ToString(),
                   };
        }

        /// <inheritdoc />
        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("[JellyfinJav] Warashi - Scanning: " + info.Name);

            Api.Actress? actress;
            if (info.ProviderIds.ContainsKey("Warashi"))
            {
                actress = await WarashiClient.LoadActress(info.ProviderIds["Warashi"]).ConfigureAwait(false);
            }
            else
            {
                actress = await WarashiClient.SearchFirst(info.Name).ConfigureAwait(false);
            }

            if (!actress.HasValue)
            {
                return new MetadataResult<Person>();
            }

            return new MetadataResult<Person>
            {
                // Changing the actress name but still keeping them associated with
                // their videos will be a challenge.
                Item = new Person
                {
                    ProviderIds = new Dictionary<string, string> { { "Warashi", actress.Value.Id } },
                    PremiereDate = actress.Value.Birthdate,
                    ProductionLocations = new[] { actress.Value.Birthplace }.Where(i => i is string).ToArray(),

                    // Jellyfin will always refresh metadata unless Overview exists.
                    // So giving Overview a zero width character to prevent that.
                    Overview = "\u200B",
                },
                HasMetadata = true,
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }
    }
}