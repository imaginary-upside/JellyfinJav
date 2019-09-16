using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace JellyfinJav.Providers.Asianscreens
{
    public class AsianscreensApi
    {
        private readonly HttpClient httpClient;
        private string html;

        public string id;

        public AsianscreensApi()
        {
            httpClient = new HttpClient();
        }

        public async Task findActress(string name)
        {
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
            var actressUrl = rx.Match(html)?.Groups[1].Value.Trim();
            id = extractId(actressUrl);
            await loadActress(id);
        }

        public async Task loadActress(string id)
        {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/{0}.asp", id)
            );
            html = await response.Content.ReadAsStringAsync();
            this.id = id;
        }

        public DateTime? getBirthdate()
        {
            var rx = new Regex("<B>DOB:.*\n.*>(.*)<\\/FONT>", RegexOptions.Compiled);
            var match = rx.Match(html)?.Groups[1].Value.Trim();

            if ("n/a" == match || String.IsNullOrEmpty(match))
            {
                return null;
            }

            return DateTime.Parse(match);
        }

        public string getCover()
        {
            var rx = new Regex("<IMG SRC=\"(.*)\" ALT=\".*'s Picture\">", RegexOptions.Compiled);
            var match = rx.Match(html)?.Groups[1].Value;
            return String.Format("https://asianscreens.com{0}", match.Trim());
        }

        public string getBirthplace()
        {
            var rx = new Regex("Birthplace:.*\n<TD><FONT .*>(.*)<\\/FONT>", RegexOptions.Compiled);
            var match = rx.Match(html);

            var birthplace = match?.Groups[1].Value.Trim();
            if (birthplace == "n/a")
            {
                return null;
            }

            return birthplace;
        }

        private static string extractId(string text)
        {
            var rx = new Regex(".com\\/(.*).asp", RegexOptions.Compiled);
            return rx.Match(text)?.Groups[1].Value.Trim();
        }
    }
}