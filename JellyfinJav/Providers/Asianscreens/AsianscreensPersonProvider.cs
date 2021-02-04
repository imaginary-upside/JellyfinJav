using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Providers;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace JellyfinJav.Providers.AsianscreensProvider
{
    public class AsianscreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly ILogger<AsianscreensPersonProvider> logger;
        private static readonly Api.AsianscreensClient client = new Api.AsianscreensClient();

        public string Name => "Asianscreens";

        public AsianscreensPersonProvider(ILogger<AsianscreensPersonProvider> logger)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            return from actress in await client.Search(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = actress.name,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Asianscreens", actress.id }
                       },
                       ImageUrl = actress.cover.ToString()
                   };
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            logger.LogInformation("[JellyfinJav] Asianscreens - Scanning: " + info.Name);

            Api.Actress? actress;
            if (info.ProviderIds.ContainsKey("Asianscreens"))
                actress = await client.LoadActress(info.ProviderIds["Asianscreens"]).ConfigureAwait(false);
            else
                actress = await client.SearchFirst(info.Name).ConfigureAwait(false);

            if (!actress.HasValue)
                return new MetadataResult<Person>();

            return new MetadataResult<Person>
            {
                // Changing the actress name but still keeping them associated with
                // their videos will be a challenge.
                Item = new Person
                {
                    ProviderIds = new Dictionary<string, string> { { "Asianscreens", actress.Value.Id } },
                    PremiereDate = actress.Value.Birthdate,
                    ProductionLocations = new [] { actress.Value.Birthplace },
                    // Jellyfin will always refresh metadata unless Overview exists.
                    // So giving Overview a zero width character to prevent that.
                    Overview = "\u200B"
                },
                HasMetadata = true
            };
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await httpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }
    }
}