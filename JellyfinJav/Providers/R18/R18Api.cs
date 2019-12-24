using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;
using System.Text.RegularExpressions;

namespace JellyfinJav.Providers.R18
{
    class R18Api
    {
        private readonly HttpClient httpClient;
        private IDocument document;
        private readonly static IDictionary<String, String> censoredWords = new Dictionary<String, String>
        {
            {"S***e", "Slave"},
            {"S*********l", "School Girl"},
            {"S********l", "Schoolgirl"},
            {"F***e", "Force"},
            {"M****t", "Molest"},
            {"S*****t", "Student"},
            {"T*****e", "Torture"},
            {"D**g", "Drug"},
            {"H*******e", "Hypnotize"},
            {"C***d", "Child"},
            {"V*****e", "Violate"},
            {"Y********l", "Young Girl"},
            {"A*****t", "Assault"},
            {"D***k", "Drink"},
            {"V*****t", "Violent"},
            {"S******g", "Sleeping"},
            {"R**e", "Rape"},
            {"S**t", "Scat"}
        };

        public R18Api()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<IEnumerable<(string, string, string)>> searchVideos(string code)
        {
            var response = await httpClient.GetAsync(
                string.Format("https://www.r18.com/common/search/order=match/searchword={0}", code)
            );
            var html = await response.Content.ReadAsStringAsync();
            var doc = await BrowsingContext.New().OpenAsync(req => req.Content(html));

            return doc.QuerySelectorAll(".item-list")
                      .Select(n =>
                      {
                          var title = n.QuerySelector("dt")?.TextContent.Trim();
                          var id = n.GetAttribute("data-content_id");
                          var cover = n.QuerySelector("img")?.GetAttribute("data-original");
                          return (title, id, cover);
                      });
        }

        public async Task loadVideo(string id)
        {
            var response = await httpClient.GetAsync(
                string.Format("https://www.r18.com/videos/vod/movies/detail/-/id={0}/", id)
            );
            var html = await response.Content.ReadAsStringAsync();
            document = await BrowsingContext.New().OpenAsync(req => req.Content(html));
        }

        public IEnumerable<string> getActresses()
        {
            return document.QuerySelectorAll("[data-type='actress-list'] a span")
                           .Select(n =>
                           {
                               var name = n.TextContent;
                               var index = name.IndexOf('(');
                               if (index != -1)
                                   return name.Substring(0, index - 1);
                               else
                                   return name;
                           });
        }

        public string getTitle()
        {
            var title = new StringBuilder(document.QuerySelector("cite[itemprop='name']")?.TextContent);

            // r18.com normally appends actress name to end of title
            foreach (var actress in getActresses())
            {
                title.Replace(actress, "");
                title.Replace(actress.ToUpper(), "");
                title.Replace(actress.ToLower(), "");
            }

            return decensor(title.ToString()).TrimEnd(' ', '-');
        }

        public IEnumerable<string> getCategories()
        {
            return document.QuerySelectorAll("[itemprop='genre']")
                           .Select(n => decensor(n.TextContent).Trim());
        }

        public DateTime? getReleaseDate()
        {
            var date = document.QuerySelector("[itemprop='dateCreated']")?
                               .TextContent.Trim();
            if (String.IsNullOrEmpty(date))
            {
                return null;
            }

            // r18.com uses non-standard abreviations which c# doesn't natively support.
            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.AbbreviatedMonthNames = new string[]
            {
                "Jan.", "Feb.", "Mar.", "Apr.", "May", "June", "July", "Aug.", "Sept.", "Oct.", "Nov.", "Dec.", ""
            };

            return DateTime.Parse(date, culture);
        }

        public string getStudio()
        {
            return document.QuerySelector("[itemprop='productionCompany'] a")?
                           .TextContent.Trim();
        }

        public string getId()
        {
            return document.QuerySelectorAll("dt")
                           .FirstOrDefault(n => n.TextContent == "Content ID:")?
                           .NextElementSibling?
                           .TextContent.Trim();
        }

        private static string decensor(string censored)
        {
            var rx = new Regex(String.Join("|", censoredWords.Keys.Select(k => Regex.Escape(k))));
            return rx.Replace(censored, m => censoredWords[m.Value]);
        }
    }
}