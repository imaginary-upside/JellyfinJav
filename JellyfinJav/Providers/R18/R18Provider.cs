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
using Microsoft.Extensions.Logging;

namespace JellyfinJav.Providers.R18 {
  public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo> {
    private readonly IServerConfigurationManager configManager;
    private readonly IHttpClient httpClient;

    public string Name => "R18";

    public R18Provider(IServerConfigurationManager configManager,
                       IHttpClient httpClient) {
      this.configManager = configManager;
      this.httpClient = httpClient;
    }

    public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                   CancellationToken cancelToken) {
      var result = new MetadataResult<Movie>();

      var r18Client = new R18Api(info.Name);
      await r18Client.init();

      if (!r18Client.successful) {
        return result;
      }

      result.Item = new Movie();
      result.Item.ProviderIds.Add("R18", r18Client.id);
      //result.Item.Name = r18Client.getTitle();
      result.HasMetadata = true;

      foreach (string category in r18Client.getCategories()) {
          result.Item.AddGenre(category);
      }

      foreach (string actress in r18Client.actresses) {
        result.AddPerson(new PersonInfo {
          Name = actress,
          Type = PersonType.Actor
        });
      }

      return result;
    }
    
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken) {
        throw new NotImplementedException();
    }
    
    public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken) {
        throw new NotImplementedException();
    }
  }
}