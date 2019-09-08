using System;
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

        private string html;

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken) {
            throw new NotImplementedException();
        }

        public Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken) {
            throw new NotImplementedException();
        }
    }
}