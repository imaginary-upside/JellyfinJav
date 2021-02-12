#pragma warning disable SA1600

namespace JellyfinJav.Providers.AsianscreensProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Providers;
    using MediaBrowser.Model.Providers;
    using Microsoft.Extensions.Logging;

    public class AsianscreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Api.AsianscreensClient Client = new Api.AsianscreensClient();
        private readonly ILogger<AsianscreensPersonProvider> logger;

        public AsianscreensPersonProvider(ILogger<AsianscreensPersonProvider> logger)
        {
            this.logger = logger;
        }

        public string Name => "Asianscreens";

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            return from actress in await Client.Search(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = actress.name,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Asianscreens", actress.id },
                       },
                       ImageUrl = actress.cover.ToString(),
                   };
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("[JellyfinJav] Asianscreens - Scanning: " + info.Name);

            Api.Actress? actress;
            if (info.ProviderIds.ContainsKey("Asianscreens"))
            {
                actress = await Client.LoadActress(info.ProviderIds["Asianscreens"]).ConfigureAwait(false);
            }
            else
            {
                actress = await Client.SearchFirst(info.Name).ConfigureAwait(false);
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
                    ProviderIds = new Dictionary<string, string> { { "Asianscreens", actress.Value.Id } },
                    PremiereDate = actress.Value.Birthdate,
                    ProductionLocations = new[] { actress.Value.Birthplace },

                    // Jellyfin will always refresh metadata unless Overview exists.
                    // So giving Overview a zero width character to prevent that.
                    Overview = "\u200B",
                },
                HasMetadata = true,
            };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await HttpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }
    }
}