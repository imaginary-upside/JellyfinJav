using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JellyfinJav.Api
{
    public class R18Client
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly IBrowsingContext context = BrowsingContext.New();
        private readonly static IDictionary<String, String> censoredWords = new Dictionary<String, String>
        {
            {"S***e", "Slave"},
            {"S*********l", "School Girl"},
            {"S********l", "Schoolgirl"},
            {"Sch**l", "School"},
            {"F***e", "Force"},
            {"F*****g", "Forcing"},
            {"P****h", "Punish"},
            {"M****t", "Molest"},
            {"S*****t", "Student"},
            {"T*****e", "Torture"},
            {"D**g", "Drug"},
            {"H*******e", "Hypnotize"},
            {"C***d", "Child"},
            {"V*****e", "Violate"},
            {"Y********l", "Young Girl"},
            {"A*****t", "Assault"},
            {"D***king", "Drinking"},
            {"D***k", "Drunk"},
            {"V*****t", "Violent"},
            {"S******g", "Sleeping"},
            {"R**e", "Rape"},
            {"R****g", "Raping"},
            {"S**t", "Scat"},
            {"K****r", "Killer"},
            {"H*******m", "Hypnotism"},
            {"G*******g", "Gangbang"},
            {"C*ck", "Cock"},
            {"K*ds", "Kids"},
            {"K****p", "Kidnap"},
            {"A****p", "Asleep"},
            {"U*********s", "Unconscious"},
            {"D******e", "Disgrace"},
            {"P********t", "Passed Out"},
            {"M************n", "Mother And Son"},
        };

        public async Task<IEnumerable<(string code, string id, Uri cover)>> Search(string searchCode)
        {
            var response = await httpClient.GetAsync(
                $"https://www.r18.com/common/search/order=match/searchword={searchCode}"
            );
            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

            return doc.QuerySelectorAll(".item-list")
                      .Select(n =>
                      {
                          var code = n.QuerySelector("img")?.GetAttribute("alt");
                          var id = n.GetAttribute("data-content_id");
                          var cover = new Uri(n.QuerySelector("img")?.GetAttribute("data-original"));
                          return (code, id, cover);
                      });
        }

        public async Task<Video?> SearchFirst(string searchCode)
        {
            var results = await Search(searchCode);
            if (results.Count() == 0)
                return null;

            return await LoadVideo(results.First().id);
        }

        public async Task<Video?> LoadVideo(string id)
        {
            var response = await httpClient.GetAsync(
                $"https://www.r18.com/videos/vod/movies/detail/-/id={id}/"
            );
            if (!response.IsSuccessStatusCode)
                return null;

            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

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
                releaseDate: releaseDate
            );
        }

        private static string NormalizeTitle(string title, IEnumerable<string> actresses)
        {
            if (actresses.Count() != 1)
                return title;

            var name = actresses.ElementAt(0);
            var rx = new Regex($@"^({name} - )?(.+?)( ?-? {name})?$");
            var match = rx.Match(title);

            if (!match.Success)
                return title;

            return match.Groups[2].Value;
        }

        public static string NormalizeActress(string actress)
        {
            var rx = new Regex(@"^(.+?)( ?\(.+\))?$");
            var match = rx.Match(actress);

            if (!match.Success)
                return actress;

            return match.Groups[1].Value;
        }

        private DateTime? ExtractReleaseDate(IDocument doc)
        {
            var release = doc.QuerySelector("[itemprop=dateCreated]")?.TextContent;
            if (release == null)
                return null;

            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.AbbreviatedMonthNames = new string[]
            {
                "Jan.", "Feb.", "Mar.", "Apr.", "May", "June", "July", "Aug.", "Sept.", "Oct.", "Nov.", "Dec.", ""
            };

            return DateTime.Parse(release, culture);
        }

        private static string Decensor(string censored)
        {
            if (censored == null)
                return censored;

            var rx = new Regex(String.Join("|", censoredWords.Keys.Select(k => Regex.Escape(k))));
            return rx.Replace(censored, m => censoredWords[m.Value]);
        }

        private static bool NotSaleGenre(string genre)
        {
            var rx = new Regex(@"\bsale\b", RegexOptions.IgnoreCase);
            var match = rx.Match(genre);

            return !match.Success;
        }
    }
}