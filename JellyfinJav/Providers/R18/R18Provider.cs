using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller.Configuration;

namespace JellyfinJav.Providers.R18
{
    public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        private readonly IServerConfigurationManager configManager;
        private readonly IHttpClient httpClient;

        public string Name => "R18";

        public R18Provider(IServerConfigurationManager configManager,
                           IHttpClient httpClient)
        {
            this.configManager = configManager;
            this.httpClient = httpClient;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                             CancellationToken cancelToken)
        {
            var client = new R18Api();
            if (info.ProviderIds.ContainsKey("R18"))
                await client.loadVideo(info.ProviderIds["R18"]);
            else
            {
                var videoResults = await GetSearchResults(info, cancelToken);
                if (videoResults.Count() == 0)
                    return new MetadataResult<Movie>();
                await client.loadVideo(videoResults.First().ProviderIds["R18"]);
            }

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = info.Name,
                    Name = client.getTitle(),
                    PremiereDate = client.getReleaseDate(),
                    ProviderIds = new Dictionary<string, string> { { "R18", client.getId() } },
                    Studios = new[] { client.getStudio() }.OfType<string>().ToArray(),
                    Genres = client.getCategories().OfType<string>().ToArray()
                },
                People = (from actress in client.getActresses()
                          select new PersonInfo
                          {
                              Name = actress,
                              Type = "JAV Actress"
                          }).ToList(),
                HasMetadata = true
            };
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            var client = new R18Api();
            return from video in await client.searchVideos(info.Name)
                   select new RemoteSearchResult
                   {
                       Name = video.Item1,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "R18", video.Item2 }
                       },
                       ImageUrl = video.Item3
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