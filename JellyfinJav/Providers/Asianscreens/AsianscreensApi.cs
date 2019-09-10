using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace JellyfinJav.Providers.Asianscreens {
    public class AsianscreensApi {
        private readonly HttpClient httpClient;
        private string html;

        public string id;

        public AsianscreensApi() {
            httpClient = new HttpClient();
        }

        public AsianscreensApi(string id) : this() {
            this.id = id;
        }

        public async Task findActress(string name) {
            var response = await httpClient.GetAsync(
                Uri.EscapeUriString(
                    String.Format("https://www.asianscreens.com/search/index.asp?zoom_query=\"{0}\"", name)
                )
            );
            // dotnet core doesnt support windows-1252 encoding, so have to do this hack
            var htmlBytes = await response.Content.ReadAsByteArrayAsync();
            html = Encoding.UTF8.GetString(htmlBytes, 0, htmlBytes.Length - 1);

            var rx = new Regex(
                "<div class=\"result_title\">.*href=\"(.*?)\".*<\\/div>",
                RegexOptions.Compiled
            );
            var actressUrl = rx.Match(html)?.Groups[1].Value;
            id = extractId(actressUrl);
            await loadActress();
        }

        public async Task loadActress() {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/{0}.asp", id)
            );
            html = await response.Content.ReadAsStringAsync();
        }

        public DateTime? getBirthdate() {
            var rx = new Regex("<B>DOB:.*\n.*>(.*)<\\/FONT>", RegexOptions.Compiled);
            var match = rx.Match(html)?.Groups[1].Value;

            if ("n/a" == match || "" == match) {
                return null;
            }

            return DateTime.Parse(match);
        }

        public string getCover() {
            var rx = new Regex("<IMG SRC=\"(.*)\" ALT=\".*'s Picture\">", RegexOptions.Compiled);
            var match = rx.Match(html)?.Groups[1].Value;
            return String.Format("https://asianscreens.com{0}", match);
        }

        private static string extractId(string text) {
            var rx = new Regex(".com\\/(.*).asp", RegexOptions.Compiled);
            return rx.Match(text)?.Groups[1].Value;
        }
    }
}