using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using AngleSharp;
using System.Linq;

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

        public async Task<bool> findActress(string name)
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
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            await loadActress(id);

            return true;
        }

        public async Task loadActress(string id)
        {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/{0}.asp", id)
            );
            html = await response.Content.ReadAsStringAsync();
            this.id = id;
        }

        public async Task<IEnumerable<(string, string)>> searchActresses(string searchName, bool reverseName = true)
        {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/directory/{0}.asp",
                              searchName.First())
            );
            html = await response.Content.ReadAsStringAsync();

            var context = BrowsingContext.New();
            var document = await context.OpenAsync(req => req.Content(html));
            var actressRows = document.QuerySelectorAll("table[bgcolor='#000000']")
                                      .First(n => n.InnerHtml.Contains("ACTRESS"))
                                      .QuerySelectorAll("tr:not(:last-child)")
                                      .Skip(2);

            var nameResults = actressRows.Select(row =>
            {
                var name = row.QuerySelector("td:nth-child(1)")?.TextContent;
                var id = row.QuerySelector("a")?
                            .GetAttribute("href")
                            .TrimStart('/')
                            .Replace(".asp", "");
                return (name: name, id: id);
            }).Where(actress => searchName
                                .ToLower()
                                .Split(' ')
                                .All(word => actress.name.ToLower().Contains(word)));

            if (reverseName)
            {
                var reverseNameResults = await searchActresses(
                    String.Join(" ", searchName.Split(' ').Reverse()), reverseName: false
                );
                return nameResults.Union(reverseNameResults).ToList();
            }
            else
            {
                return nameResults;
            }
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