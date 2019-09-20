using System;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using System.Collections.Generic;
using MediaBrowser.Common.Net;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Providers;
using System.Linq;

namespace JellyfinJav.Providers.Asianscreens
{
    public class AsianscreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private readonly IHttpClient httpClient;

        public string Name => "Asianscreens";

        public AsianscreensPersonProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            var client = new AsianscreensApi();
            return from actress in await client.searchActresses(info.Name)
                   select new RemoteSearchResult
                   {
                       Name = actress.Item1,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Asianscreens", actress.Item2 }
                       }
                   };
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Person>();

            var client = new AsianscreensApi();

            if (info.ProviderIds.ContainsKey("Asianscreens"))
            {
                return result;
            }

            await client.findActress(info.Name);

            result.Item = new Person
            {
                Name = info.Name,
                PremiereDate = client.getBirthdate()
            };
            result.Item.ProviderIds.Add("Asianscreens", client.id);
            result.HasMetadata = true;

            var birthplace = client.getBirthplace();
            if (birthplace != null)
            {
                result.Item.ProductionLocations = new string[] { client.getBirthplace() };
            }

            return result;
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }
    }
}