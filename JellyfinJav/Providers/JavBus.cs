using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyfinJav.JellyfinJav.Providers
{
    public class JavBus
    {
        public static async Task<IEnumerable<JavBusResult>> GetAllResults(IHttpClient httpClient, ILogger logger, string name)
        {
            logger.LogInformation($"Jav find movies {name}");
            try
            {
                var res = await httpClient.GetResponse(new HttpRequestOptions
                {
                    Url = $"https://www.javbus.com/search/{name}"
                });

                var html = await new StreamReader(res.Content).ReadToEndAsync();
                var doc = await BrowsingContext.New().OpenAsync(req => req.Content(html));

                var ret = from element in doc.QuerySelectorAll(".movie-box")
                    select new JavBusResult
                    {
                        Name = element.QuerySelector("img").GetAttribute("title"),
                        Url = element.GetAttribute("href"),
                        ImageUrl = element.QuerySelector("img").GetAttribute("src"),
                        Code = GetCodeFromUrl(element.GetAttribute("href"))
                    };

                return ret;
            }
            catch (Exception e)
            {
                logger.LogInformation($"Jav Search movies {name} exceptions. {e.Message}");
                return new JavBusResult[0];
            }
        }

        private static string GetCodeFromUrl(string url)
        {
            var parts = url.Split('/');
            return parts.Last();
        }

        public static async Task<JavBusResult> GetResult(IHttpClient httpClient, ILogger logger, string code)
        {
            logger.LogInformation("Jav Load movie " + code);
            var res = await httpClient.GetResponse(new HttpRequestOptions
            {
                Url = $"https://www.javbus.com/{code}"
            });

            var html = await new StreamReader(res.Content).ReadToEndAsync();
            var doc = await BrowsingContext.New().OpenAsync(req => req.Content(html));

            var image = doc.QuerySelector(".container .screencap img");

            var ret = new JavBusResult
            {
                Code = code,
                Url = res.ResponseUrl,
                Name = image.GetAttribute("title"),
                ImageUrl = image.GetAttribute("src"),
                Actresses = from e in doc.QuerySelectorAll(".container .star-box a img")
                    select new Actress
                    {
                        Name = e.GetAttribute("title"),
                        ImageUrl = e.GetAttribute("src")
                    },
                Genres = from e in doc.QuerySelectorAll(".container .genre a") select e.TextContent
            };

            logger.LogInformation(
                $"Jav Load movie(code={code}, title={ret.Name}), genres={string.Join(",", ret.Genres)}");

            return ret;
        }

        public static MetadataResult<Movie> GetMovieFromResult(string oldName, JavBusResult result)
        {
            return new MetadataResult<Movie>
            {
                HasMetadata = true,
                Item = new Movie
                {
                    OriginalTitle = oldName,
                    Name = result.Name,
                    ProviderIds = new Dictionary<string, string> {{"JavBus", result.Code}},
                    Genres = result.Genres.ToArray()
                },
                People = (from actress in result.Actresses
                    select new PersonInfo
                    {
                        Name = actress.Name,
                        Type = "JAV Actress",
                        ImageUrl = actress.ImageUrl
                    }).ToList()
            };
        }
    }

    public class JavBusExternalId : IExternalId
    {
        public string Name => "JavBus";

        public string Key => "JavBus";

        public string UrlFormatString => "http://javbus.com/{0}";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }

    public class JavBusImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;

        public JavBusImageProvider(IHttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public bool Supports(BaseItem item)
        {
            return item is Movie;
        }

        public string Name => "JavBus";

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] {ImageType.Primary};
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Jav Get image for {item.Name}");
            if (item.ProviderIds.ContainsKey("JavBus"))
            {
                var code = item.ProviderIds["JavBus"];
                var ret = from result in await JavBus.GetAllResults(httpClient, logger, code)
                    select new RemoteImageInfo
                    {
                        ProviderName = "JavBus",
                        Type = ImageType.Primary,
                        Url = result.ImageUrl
                    };

                return ret;
            }

            return new RemoteImageInfo[0];
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancellationToken
            });
        }
    }

    public class JavBusMetadataProvider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        private readonly IServerConfigurationManager configManager;
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;

        public JavBusMetadataProvider(IServerConfigurationManager configManager,
            IHttpClient httpClient, ILogger logger)
        {
            this.configManager = configManager;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public string Name => "JavBus";

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancellationToken
            });
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
        {
            if (info.ProviderIds.ContainsKey("JavBus"))
            {
                logger.LogInformation("Jav Get metadata with code " + info.ProviderIds["JavBus"]);
                return JavBus.GetMovieFromResult(info.Name,
                    await JavBus.GetResult(httpClient, logger, info.ProviderIds["JavBus"]));
            }

            logger.LogInformation($"Jav Get metadata with name {info.Name}");

            var results = await GetSearchResults(info, cancellationToken);
            try
            {
                var first = results.First();
                return JavBus.GetMovieFromResult(info.Name, await JavBus.GetResult(httpClient, logger, first.ProviderIds["JavBus"]));
            }
            catch (Exception e)
            {
                logger.LogInformation($"Jav Get metadata no result for {info.Name}, {e.Message}");
                return new MetadataResult<Movie>();
            }
        }


        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo,
            CancellationToken cancellationToken)
        {
            if (!searchInfo.ProviderIds.ContainsKey("JavBus") || string.IsNullOrWhiteSpace(searchInfo.ProviderIds["JavBus"]))
            {
                logger.LogInformation($"Jav find movie with name {searchInfo.Name}");
                var code = searchInfo.Name.Split(' ').First();
                return from e in await JavBus.GetAllResults(httpClient, logger, code)
                    select new RemoteSearchResult
                    {
                        SearchProviderName = "JavBus",
                        Name = e.Name,
                        ImageUrl = e.ImageUrl,
                        ProviderIds = new Dictionary<string, string> {{"JavBus", e.Code}}
                    };
            }

            logger.LogInformation($"Jav find movie with id {searchInfo.ProviderIds["JavBus"]}");
            var result = await JavBus.GetResult(httpClient, logger, searchInfo.ProviderIds["JavBus"]);
            return new[]
            {
                new RemoteSearchResult
                {
                    SearchProviderName = "JavBus",
                    Name = result.Name,
                    ImageUrl = result.ImageUrl,
                    ProviderIds = new Dictionary<string, string> {{"JavBus", result.Code}}
                }
            };
        }
    }

    public class JavBusResult
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<Actress> Actresses { set; get; }

        public IEnumerable<string> Genres { set; get; }
    }

    public class Actress
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}