using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller.Configuration;

namespace JellyfinJav.Providers.Javlibrary {
  public class JavlibraryProvider : IRemoteMetadataProvider<Movie, MovieInfo> {
    private readonly IServerConfigurationManager configManager;
    private readonly IHttpClient httpClient;

    public string Name => "Javlibrary";

    public JavlibraryProvider(IServerConfigurationManager configManager,
                              IHttpClient httpClient) {
      this.configManager = configManager;
      this.httpClient = httpClient;
    }

    public Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                   CancellationToken cancelToken) {
      var result = new MetadataResult<Movie>();

      result.Item = new Movie {
        ProductionLocations = new string[] {"Japan"},
        ProductionYear = 2018,
        OfficialRating = "3.8",
        Overview = "Fuck you so much"
      };
      result.Item.ProviderIds.Add("Javlibrary", "2");
      result.HasMetadata = true;

      var personInfo = new PersonInfo {
        Name = "Arina Hashimoto",
        Type = PersonType.Actor,
        Role = string.Empty
      };
      result.AddPerson(personInfo);

      return Task.FromResult<MetadataResult<Movie>>(result);
    }
    
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info,
                                                                  CancellationToken cancelToken) {
      var results = new List<RemoteSearchResult>();

      results.Add(new RemoteSearchResult {
        Name = "testing",
        SearchProviderName = "testing",
        ImageUrl = "https://m.worldanimalfoundation.net/i/pig3s.jpg"
      });
      results[0].SetProviderId("Javlibrary", "2");

      return Task.FromResult<IEnumerable<RemoteSearchResult>>(results);
    }
    
    public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken) {
        return Task.FromResult<HttpResponseInfo>(new HttpResponseInfo());
    }
  }
}
