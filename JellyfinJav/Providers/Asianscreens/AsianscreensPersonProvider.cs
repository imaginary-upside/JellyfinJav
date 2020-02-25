using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using System.Collections.Generic;
using MediaBrowser.Common.Net;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Providers;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace JellyfinJav.Providers.AsianscreensProvider
{
    public class AsianscreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;
        private static readonly Api.AsianscreensClient client = new Api.AsianscreensClient();

        public string Name => "Asianscreens";

        public AsianscreensPersonProvider(IHttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            return from actress in await client.Search(info.Name)
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

            Api.Actress? actress = null;
            if (info.ProviderIds.ContainsKey("Asianscreens"))
                actress = await client.LoadActress(info.ProviderIds["Asianscreens"]);
            else
                actress = await client.SearchFirst(info.Name);

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
                    ProductionLocations = new[] { actress.Value.Birthplace }.OfType<string>().ToArray(),
                    // Jellyfin will always refresh metadata unless Overview exists.
                    // So giving Overview a zero width character to prevent that.
                    Overview = "\u200B"
                },
                HasMetadata = true
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