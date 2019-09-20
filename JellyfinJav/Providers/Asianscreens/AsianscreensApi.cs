using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Dom;
using System.Linq;

namespace JellyfinJav.Providers.Asianscreens
{
    public class AsianscreensApi
    {
        private readonly HttpClient httpClient;
        private IDocument document;

        public AsianscreensApi()
        {
            httpClient = new HttpClient();
        }

        public async Task loadActress(string id)
        {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/{0}.asp", id)
            );
            var html = await response.Content.ReadAsStringAsync();
            document = await BrowsingContext.New()
                                 .OpenAsync(req => req.Content(html));
        }

        public async Task<IEnumerable<(string, string)>> searchActresses(string searchName, bool reverseName = true)
        {
            var response = await httpClient.GetAsync(
                String.Format("https://www.asianscreens.com/directory/{0}.asp",
                              searchName.First())
            );
            var html = await response.Content.ReadAsStringAsync();

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
            var birthdate = document.QuerySelectorAll("td")
                            .First(n => n.TextContent == "DOB: ")
                            .NextElementSibling
                            .TextContent;

            if ("n/a" == birthdate)
            {
                return null;
            }

            return DateTime.Parse(birthdate);
        }

        public string getBirthplace()
        {
            var birthplace = document.QuerySelectorAll("td")
                            .First(n => n.TextContent == "Birthplace: ")
                            .NextElementSibling
                            .TextContent;

            if (birthplace == "n/a")
            {
                return null;
            }

            return birthplace;
        }
    }
}