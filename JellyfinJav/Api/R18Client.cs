namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;

    /// <summary>A web scraping client for r18.com.</summary>
    public static class R18Client
    {
        private static readonly IDictionary<string, string> CensoredWords = new Dictionary<string, string>
        {
            { "S***e", "Slave" },
            { "S*********l", "School Girl" },
            { "S********l", "Schoolgirl" },
            { "Sch**l", "School" },
            { "F***e", "Force" },
            { "F*****g", "Forcing" },
            { "P****h", "Punish" },
            { "M****t", "Molest" },
            { "S*****t", "Student" },
            { "T*****e", "Torture" },
            { "D**g", "Drug" },
            { "H*******e", "Hypnotize" },
            { "C***d", "Child" },
            { "V*****e", "Violate" },
            { "Y********l", "Young Girl" },
            { "A*****t", "Assault" },
            { "D***king", "Drinking" },
            { "D***k", "Drunk" },
            { "V*****t", "Violent" },
            { "S******g", "Sleeping" },
            { "R**e", "Rape" },
            { "R****g", "Raping" },
            { "S**t", "Scat" },
            { "K****r", "Killer" },
            { "H*******m", "Hypnotism" },
            { "G*******g", "Gangbang" },
            { "C*ck", "Cock" },
            { "K*ds", "Kids" },
            { "K****p", "Kidnap" },
            { "A****p", "Asleep" },
            { "U*********s", "Unconscious" },
            { "D******e", "Disgrace" },
            { "P********t", "Passed Out" },
            { "M************n", "Mother And Son" },
        };

        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly IBrowsingContext Context = BrowsingContext.New();

        static R18Client()
        {
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        /// <summary>Searches for a video by jav code.</summary>
        /// <param name="searchCode">The jav code. Ex: ABP-001.</param>
        /// <returns>A list of every matched video.</returns>
        public static async Task<IEnumerable<VideoResult>> Search(string searchCode)
        {
            var response = await HttpClient.GetAsync($"https://www.r18.com/common/search/order=match/searchword={searchCode}").ConfigureAwait(false);
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var videos = new List<VideoResult>();

            foreach (var n in doc.QuerySelectorAll(".item-list"))
            {
                var code = n.QuerySelector("img")?.GetAttribute("alt");
                var id = n.GetAttribute("data-content_id");
                var cover = n.QuerySelector("img")?.GetAttribute("data-original");

                if (code is not null && cover is not null)
                {
                    videos.Add(new VideoResult
                    {
                        Code = code,
                        Id = id,
                        Cover = new Uri(cover),
                    });
                }
            }

            return videos;
        }

        /// <summary>Searches for a video by jav code, and returns the first result.</summary>
        /// <param name="searchCode">The jav code. Ex: ABP-001.</param>
        /// <returns>The parsed video.</returns>
        public static async Task<Video?> SearchFirst(string searchCode)
        {
            var results = await Search(searchCode).ConfigureAwait(false);
            if (!results.Any())
            {
                return null;
            }

            // See if we can find an exact match first.
            foreach (var result in results)
            {
                if (string.Equals(searchCode, result.Code))
                {
                    return await LoadVideo(result.Id).ConfigureAwait(false);
                }
            }

            return await LoadVideo(results.First().Id).ConfigureAwait(false);
        }

        /// <summary>Loads a video by id.</summary>
        /// <param name="id">The r18.com unique video identifier.</param>
        /// <returns>The parsed video.</returns>
        public static async Task<Video?> LoadVideo(string id)
        {
            var response = await HttpClient.GetAsync($"https://www.r18.com/api/v4f/contents/{id}").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var res = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var videoResponse = JsonSerializer.Deserialize<VideoResponse>(res, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            var code = videoResponse?.Data?.Code;
            var title = Decensor(videoResponse?.Data?.Title);
            var actresses = videoResponse?.Data?.Actresses?.Select(a => NormalizeActress(a.Name)) ?? Array.Empty<string>();
            var genres = videoResponse?.Data?.Categories?.Select(genre => Decensor(genre.Name)).OfType<string>().Where(genre => NotSaleGenre(genre)) ?? Array.Empty<string>();
            var studio = videoResponse?.Data?.Maker?.Name;
            var boxArt = videoResponse?.Data?.Images?.JacketImage?.Large;
            var cover = videoResponse?.Data?.Images?.JacketImage?.Medium;
            DateTime? releaseDate = null;
            if (!string.IsNullOrEmpty(videoResponse?.Data?.ReleaseDate))
            {
                releaseDate = DateTime.Parse(videoResponse.Data.ReleaseDate);
            }

            if (title is null || code is null)
            {
                return null;
            }

            title = NormalizeTitle(title, actresses);

            return new Video(
                id: id,
                code: code,
                title: title,
                actresses: actresses,
                genres: genres,
                studio: studio,
                boxArt: boxArt,
                cover: cover,
                releaseDate: releaseDate);
        }

        private static string NormalizeActress(string? actress)
        {
            if (actress is null)
            {
                return string.Empty;
            }

            var rx = new Regex(@"^(.+?)( ?\(.+\))?$");
            var match = rx.Match(actress);

            if (!match.Success)
            {
                return actress;
            }

            return match.Groups[1].Value;
        }

        private static string NormalizeTitle(string title, IEnumerable<string> actresses)
        {
            if (actresses.Count() != 1)
            {
                return title;
            }

            var name = actresses.ElementAt(0);
            var rx = new Regex($"^({name} - )?(.+?)( ?-? {name})?$");
            var match = rx.Match(title);

            if (!match.Success)
            {
                return title;
            }

            return match.Groups[2].Value;
        }

        private static string? Decensor(string? censored)
        {
            if (censored is null)
            {
                return null;
            }

            var rx = new Regex(string.Join("|", CensoredWords.Keys.Select(k => Regex.Escape(k))));
            return rx.Replace(censored, m => CensoredWords[m.Value]);
        }

        private static bool NotSaleGenre(string genre)
        {
            var rx = new Regex(@"\bsale\b", RegexOptions.IgnoreCase);
            var match = rx.Match(genre ?? string.Empty);

            return !match.Success;
        }

        private class VideoResponse
        {
            public DataC? Data { get; set; }

            public class DataC
            {
                [JsonPropertyName("dvd_id")]
                public string? Code { get; set; }

                public string? Title { get; set; }

                [JsonPropertyName("release_date")]
                public string? ReleaseDate { get; set; }

                public Actress[] Actresses { get; set; } = Array.Empty<Actress>();

                public Image? Images { get; set; }

                public Category[] Categories { get; set; } = Array.Empty<Category>();

                public string? Studio { get; set; }

                public MakerC? Maker { get; set; }

                public class Actress
                {
                    public string? Name { get; set; }
                }

                public class Image
                {
                    [JsonPropertyName("jacket_image")]
                    public JacketImageC? JacketImage { get; set; }

                    public class JacketImageC
                    {
                        public string? Large { get; set; }

                        public string? Medium { get; set; }
                    }
                }

                public class Category
                {
                    public string? Name { get; set; }
                }

                public class MakerC
                {
                    public string? Name { get; set; }
                }
            }
        }
    }
}