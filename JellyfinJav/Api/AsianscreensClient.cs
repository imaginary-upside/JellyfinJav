using System.Text.RegularExpressions;
using System;
using AngleSharp;
using AngleSharp.Dom;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

namespace JellyfinJav.Api
{
    public class AsianscreensClient
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly IBrowsingContext context = BrowsingContext.New();
        private static readonly Regex pIdFromUrl = new Regex(@".*\/(.*)\.asp", RegexOptions.Compiled);

        public async Task<IEnumerable<(string name, string id, Uri cover)>> Search(string searchName)
        {
            var reversedName = String.Join(" ", searchName.Split(' ').Reverse());
            if (searchName != reversedName)
                return (await SearchHelper(searchName)).Concat(await SearchHelper(reversedName));
            else
                return await SearchHelper(searchName);
        }

        private async Task<IEnumerable<(string name, string id, Uri cover)>> SearchHelper(string searchName)
        {
            var response = await httpClient.GetAsync(
                $"http://www.asianscreens.com/directory/{searchName[0]}.asp"
            );

            if (!response.IsSuccessStatusCode)
                return new (string, string, Uri)[] { };

            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

            var actressRows = doc.QuerySelectorAll("table[bgcolor='#000000']")
                                 .First(n => n.InnerHtml.Contains("ACTRESS"))
                                 .QuerySelectorAll("tr:not(:last-child)")
                                 .Skip(2);

            return actressRows.Select(row =>
            {
                var name = row.QuerySelector("td:nth-child(1)")?.TextContent;
                var id = row.QuerySelector("a")?
                            .GetAttribute("href")
                            .TrimStart('/')
                            .Replace(".asp", "");
                var cover = GenerateCoverUrl(id);
                return (name: name, id: id, cover: cover);
            }).Where(actress =>
            {
                return actress.name.Split(' ').Contains(
                    searchName, StringComparer.CurrentCultureIgnoreCase
                ) || Regex.Replace(actress.name, @" #\d", "") == searchName;
            });
        }

        public async Task<Actress?> SearchFirst(string searchName)
        {
            var result = await Search(searchName);

            if (result.Count() == 0)
                return null;

            return await LoadActress(result.ElementAt(0).id);
        }

        public async Task<Actress?> LoadActress(string id)
        {
            return await LoadActress(new Uri($"http://www.asianscreens.com/{id}.asp"));
        }

        public async Task<Actress?> LoadActress(Uri url)
        {
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));

            var id = ExtractId(url);
            var name = ExtractCell(doc, "Name: ");
            var birthdate = ExtractBirthdate(doc);
            var birthplace = ExtractCell(doc, "Birthplace: ");
            var cover = GetCover(doc);

            return new Actress(
                id: id,
                name: name,
                birthdate: birthdate,
                birthplace: birthplace,
                cover: cover
            );
        }

        private static string ExtractId(Uri url)
        {
            var match = pIdFromUrl.Match(url.ToString());

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        private static DateTime? ExtractBirthdate(IDocument doc)
        {
            var asString = ExtractCell(doc, "DOB: ");

            if (asString == null)
                return null;

            return DateTime.Parse(asString);
        }

        private static string ExtractCell(IDocument doc, string cellName)
        {
            var cell = doc.QuerySelectorAll("td")
                          .FirstOrDefault(n => n.TextContent == cellName)
                          .NextElementSibling
                          .TextContent;

            if (cell == "n/a")
                return null;

            return cell;
        }

        private static string GetCover(IDocument doc)
        {
            var path = doc.QuerySelector("img[src*=\"/products/400000/portraits/\"]")
                          .GetAttribute("src");

            if (path == "/products/400000/portraits/no_picture_available.gif")
                return null;

            return $"http://www.asianscreens.com{path}";
        }

        private static Uri GenerateCoverUrl(string id)
        {
            var idEnd = id[id.Length - 1];
            var picEnd = "";
            if (idEnd == '2')
                picEnd = "";
            else
                picEnd = (Char.GetNumericValue(idEnd) - 1).ToString();

            var url = string.Format(
                "http://www.asianscreens.com/products/400000/portraits/{0}{1}.jpg",
                id.TrimEnd(idEnd),
                picEnd
            );

            return new Uri(url);
        }
    }
}