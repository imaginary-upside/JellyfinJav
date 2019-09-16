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

namespace JellyfinJav.Providers.R18
{
    public class R18Provider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        private readonly IServerConfigurationManager configManager;
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;

        public string Name => "R18";

        public R18Provider(IServerConfigurationManager configManager,
                           IHttpClient httpClient,
                           ILogger logger)
        {
            this.configManager = configManager;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
                                                       CancellationToken cancelToken)
        {
            logger.LogInformation("[JellyfinJav] Scanning: " + info.Name);

            var result = new MetadataResult<Movie>();

            var r18Client = new R18Api();
            if (info.ProviderIds.ContainsKey("R18"))
            {
                await r18Client.loadVideo(info.ProviderIds["R18"]);
            }
            else
            {
                if (!await r18Client.findVideo(info.Name))
                {
                    return result;
                }
            }

            result.Item = new Movie
            {
                OriginalTitle = info.Name,
                Name = r18Client.getTitle(),
                PremiereDate = r18Client.getReleaseDate()
            };
            result.Item.ProviderIds.Add("R18", r18Client.id);
            result.HasMetadata = true;

            if (!String.IsNullOrEmpty(r18Client.getStudio()))
            {
                result.Item.AddStudio(r18Client.getStudio());
            }

            foreach (string category in r18Client.getCategories())
            {
                result.Item.AddGenre(category);
            }

            foreach (string actress in r18Client.getActresses())
            {
                result.AddPerson(new PersonInfo
                {
                    Name = actress,
                    Type = PersonType.Actor
                });
            }

            return result;
        }

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }
    }
}