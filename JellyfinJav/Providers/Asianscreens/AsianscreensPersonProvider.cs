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

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken) {
            throw new NotImplementedException();
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken) {
            var result = new MetadataResult<Person>();

            if (info.ProviderIds.ContainsKey("Asianscreens")) {
                return result;
            }

            var asianscreensClient = new AsianscreensApi();
            await asianscreensClient.findActress(info.Name);

            result.Item = new Person {
                Name = info.Name,
                PremiereDate = asianscreensClient.getBirthdate()
            };
            result.Item.ProviderIds.Add("Asianscreens", asianscreensClient.id);
            result.HasMetadata = true;

            return result;
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken) {
            throw new NotImplementedException();
        }
    }
}