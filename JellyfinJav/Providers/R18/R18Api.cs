using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JellyfinJav.Providers.R18
{
    class R18Api
    {
        private readonly HttpClient httpClient;
        private string html;

        public string id;
        public bool successful = false;

        public R18Api()
        {
            this.httpClient = new HttpClient();
        }

        public async Task findVideo(string code)
        {
            id = await getProductId(code);
            await loadVideo(id);
        }

        public async Task loadVideo(string id)
        {
            this.id = id;

            var response = await httpClient.GetAsync(
                string.Format("https://www.r18.com/videos/vod/movies/detail/-/id={0}/", id)
            );
            html = await response.Content.ReadAsStringAsync();
        }

        public IEnumerable<string> getActresses()
        {
            var actresses = new List<string>();

            var rx = new Regex("<span itemprop=\"name\">(.*)<\\/span>", RegexOptions.Compiled);
            var matches = rx.Matches(html);

            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value;

                // removes extra names
                var index = name.IndexOf('(');
                if (index != -1)
                {
                    name = name.Substring(0, index - 1);
                }

                actresses.Add(name.Trim());
            }

            return actresses;
        }

        public string getTitle()
        {
            var rx = new Regex("<cite itemprop=\"name\">(.*)<\\/cite>", RegexOptions.Compiled);
            var match = rx.Match(html);

            var title = new StringBuilder(match?.Groups[1].Value);

            // r18.com normally appends actress name to end of title
            foreach (var actress in getActresses())
            {
                title.Replace(actress, "");
                title.Replace(actress.ToUpper(), "");
                title.Replace(actress.ToLower(), "");
            }

            return title.ToString().Trim();
        }

        public IEnumerable<string> getCategories()
        {
            var rx = new Regex("itemprop=\"genre\">\\s+(.*?)<\\/a>", RegexOptions.Compiled);
            var matches = rx.Matches(html);

            return (from Match m in matches select m.Groups[1].Value.Trim());
        }

        public DateTime? getReleaseDate()
        {
            var rx = new Regex("<dd itemprop=\"dateCreated\">(.*\n.*)<br>", RegexOptions.Compiled);
            var match = rx.Match(html);

            // r18.com uses non-standard abreviations which c# doesn't natively support.
            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.AbbreviatedMonthNames = new string[]
            {
                "Jan.", "Feb.", "Mar.", "Apr.", "May", "June", "July", "Aug.", "Sept.", "Oct.", "Nov.", "Dec.", ""
            };

            try
            {
                return DateTime.Parse(match?.Groups[1].Value.Trim(), culture);
            }
            catch
            {
                return null;
            }
        }

        public string getStudio()
        {
            var rx = new Regex("<a .* itemprop=\"name\">\\s*(.*)<\\/a>", RegexOptions.Compiled);
            var match = rx.Match(html);

            return match?.Groups[1].Value.Trim();
        }

        private async Task<string> getProductId(string code)
        {
            var response = await httpClient.GetAsync(
                String.Format("http://www.r18.com/common/search/order=match/searchword={0}", code)
            );
            var html = await response.Content.ReadAsStringAsync();

            var rx = new Regex("data-content_id=\"(.*)\"", RegexOptions.Compiled);
            var match = rx.Match(html);

            return match?.Groups[1].Value;
        }
    }
}