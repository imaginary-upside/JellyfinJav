namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

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

        /// <summary>Searches for a video by jav code.</summary>
        /// <param name="searchCode">The jav code. Ex: ABP-001.</param>
        /// <returns>A list of every matched video.</returns>
        public static async Task<IEnumerable<(string code, string id, Uri cover)>> Search(string searchCode)
        {
            var response = await HttpClient.GetAsync($"https://www.r18.com/common/search/order=match/searchword={searchCode}").ConfigureAwait(false);
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            return doc.QuerySelectorAll(".item-list")
                      .Select(n =>
                      {
                          var code = n.QuerySelector("img")?.GetAttribute("alt");
                          var id = n.GetAttribute("data-content_id");
                          var cover = new Uri(n.QuerySelector("img")?.GetAttribute("data-original"));
                          return (code, id, cover);
                      });
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
            foreach ((string code, string id, Uri _) in results)
            {
                if (string.Equals(searchCode, code))
                {
                    return await LoadVideo(id).ConfigureAwait(false);
                }
            }

            return await LoadVideo(results.First().id).ConfigureAwait(false);
        }

        /// <summary>Loads a video by id.</summary>
        /// <param name="id">The r18.com unique video identifier.</param>
        /// <returns>The parsed video.</returns>
        public static async Task<Video?> LoadVideo(string id)
        {
            var response = await HttpClient.GetAsync($"https://www.r18.com/videos/vod/movies/detail/-/id={id}/").ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var code = doc.QuerySelector(".product-details dl:nth-child(2) dd:nth-of-type(3)")?.TextContent.Trim();
            var title = Decensor(doc.QuerySelector("cite[itemprop=name]")?.TextContent);
            var actresses = doc.QuerySelectorAll("span[itemprop=name]")
                               ?.Select(n => NormalizeActress(n.TextContent))
                               .Where(actress => actress != "----");
            var genres = doc.QuerySelectorAll("[itemprop=genre]")
                               ?.Select(n => Decensor(n.TextContent.Trim()))
                               .Where(genre => NotSaleGenre(genre));
            var studio = doc.QuerySelector("[itemprop=productionCompany]")?.TextContent.Trim();
            var cover = doc.QuerySelector("[itemprop=image]")?.GetAttribute("src");
            var boxArt = cover.Replace("ps.jpg", "pl.jpg");
            var releaseDate = ExtractReleaseDate(doc);

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

        private static string NormalizeActress(string actress)
        {
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

        private static DateTime? ExtractReleaseDate(IDocument doc)
        {
            var release = doc.QuerySelector("[itemprop=dateCreated]")?.TextContent;
            if (release == null)
            {
                return null;
            }

            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.AbbreviatedMonthNames = new string[]
            {
                "Jan.", "Feb.", "Mar.", "Apr.", "May", "June", "July", "Aug.", "Sept.", "Oct.", "Nov.", "Dec.", string.Empty,
            };

            return DateTime.Parse(release, culture);
        }

        private static string Decensor(string censored)
        {
            if (censored == null)
            {
                return censored;
            }

            var rx = new Regex(string.Join("|", CensoredWords.Keys.Select(k => Regex.Escape(k))));
            return rx.Replace(censored, m => CensoredWords[m.Value]);
        }

        private static bool NotSaleGenre(string genre)
        {
            var rx = new Regex(@"\bsale\b", RegexOptions.IgnoreCase);
            var match = rx.Match(genre);

            return !match.Success;
        }
    }
}