using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JellyfinJav.Providers.R18 {
    class R18Api {
        private readonly string code;
        private readonly HttpClient httpClient;
        private string html;

        public string id;
        public readonly List<string> actresses;
        public bool successful = false;

        public R18Api(string code) {
            this.code = code;
            this.httpClient = new HttpClient();
            this.actresses = new List<string>();
        }

        public async Task init() {
            id = await getProductId();

            var response = await httpClient.GetAsync(buildUrl());
            html = await response.Content.ReadAsStringAsync();
            getActresses();

            successful = true;
        }

        private string buildUrl() {
            return String.Format(
                "https://www.r18.com/videos/vod/movies/detail/-/id={0}/", id
            );
        }

        private void getActresses() {
            var rx = new Regex("<span itemprop=\"name\">(.*)<\\/span>", RegexOptions.Compiled);
            var matches = rx.Matches(html);

            foreach (Match match in matches) {
                var name = match.Groups[1].Value;

                // removes extra names
                var index = name.IndexOf('(');
                if (index != -1) {
                    name = name.Substring(0, index - 1);
                }

                actresses.Add(name);
            }
        }

        public string getTitle() {
            var rx = new Regex("<cite itemprop=\"name\">(.*)<\\/cite>", RegexOptions.Compiled);
            var match = rx.Match(html);

            var title = new StringBuilder(match?.Groups[1].Value);

            foreach (var actress in actresses) {
                title.Replace(actress, "");
            }

            return title.ToString();
        }

        public IEnumerable<string> getCategories() {
            var rx = new Regex("itemprop=\"genre\">\\s+(.*?)<\\/a>", RegexOptions.Compiled);
            var matches = rx.Matches(html);
            
            return (from Match m in matches select m.Groups[1].Value);
        }

        private async Task<string> getProductId() {
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