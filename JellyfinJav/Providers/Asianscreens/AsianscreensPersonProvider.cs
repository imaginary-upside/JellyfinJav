using System;
using System.Linq;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using System.Collections.Generic;
using MediaBrowser.Common.Net;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.Asianscreens {
    public class AsianscreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo> {
        private readonly IHttpClient httpClient;
        
        public string Name => "Asianscreens";

        public AsianscreensPersonProvider(IHttpClient httpClient) {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken) {
            var result = new RemoteSearchResult();
            
            var client = new AsianscreensApi();
            if (info.ProviderIds.ContainsKey("Asianscreens")) {
                await client.loadActress(info.ProviderIds["Asianscreens"]);
            } else {
                await client.findActress(info.Name);
            }

            result.ProviderIds.Add("Asianscreens", client.id);
            result.PremiereDate = client.getBirthdate();
            result.Name = info.Name;
            
            return new RemoteSearchResult[] { result };
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken) {
            var result = new MetadataResult<Person>();

            var client = new AsianscreensApi();

            if (info.ProviderIds.ContainsKey("Asianscreens")) {
                return result;
            }
            
            await client.findActress(info.Name);

            result.Item = new Person {
                Name = info.Name,
                PremiereDate = client.getBirthdate()
            };
            result.Item.ProviderIds.Add("Asianscreens", client.id);
            result.HasMetadata = true;

            return result;
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken) {
            throw new NotImplementedException();
        }
    }
}