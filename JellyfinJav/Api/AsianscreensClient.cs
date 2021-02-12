#pragma warning disable SA1600

namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public class AsianscreensClient
    {
        private static readonly Regex IdFromUrl = new Regex(@".*\/(.*)\.asp", RegexOptions.Compiled);
        private readonly HttpClient httpClient = new HttpClient();
        private readonly IBrowsingContext context = BrowsingContext.New();

        public async Task<IEnumerable<(string name, string id, Uri cover)>> Search(string searchName)
        {
            var reversedName = string.Join(" ", searchName.Split(' ').Reverse());
            if (searchName != reversedName)
            {
                var first = await this.SearchHelper(searchName).ConfigureAwait(false);
                var second = await this.SearchHelper(reversedName).ConfigureAwait(false);
                return first.Concat(second);
            }
            else
            {
                return await this.SearchHelper(searchName).ConfigureAwait(false);
            }
        }

        public async Task<Actress?> SearchFirst(string searchName)
        {
            var result = await this.Search(searchName).ConfigureAwait(false);

            if (!result.Any())
            {
                return null;
            }

            return await this.LoadActress(result.ElementAt(0).id).ConfigureAwait(false);
        }

        public async Task<Actress?> LoadActress(string id)
        {
            return await this.LoadActress(new Uri($"https://www.asianscreens.com/{id}.asp")).ConfigureAwait(false);
        }

        public async Task<Actress?> LoadActress(Uri url)
        {
            var response = await this.httpClient.GetAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await this.context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

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
                cover: cover);
        }

        private static string ExtractId(Uri url)
        {
            var match = IdFromUrl.Match(url.ToString());

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        private static DateTime? ExtractBirthdate(IDocument doc)
        {
            var asString = ExtractCell(doc, "DOB: ");

            if (asString == null)
            {
                return null;
            }

            return DateTime.Parse(asString);
        }

        private static string ExtractCell(IDocument doc, string cellName)
        {
            var cell = doc.QuerySelectorAll("td")
                          .FirstOrDefault(n => n.TextContent == cellName)
                          .NextElementSibling
                          .TextContent;

            if (cell == "n/a")
            {
                return null;
            }

            return cell;
        }

        private static string GetCover(IDocument doc)
        {
            var path = doc.QuerySelector("img[src*=\"/products/400000/portraits/\"]")
                          .GetAttribute("src");

            if (path == "/products/400000/portraits/no_picture_available.gif")
            {
                return null;
            }

            return $"https://www.asianscreens.com{path}";
        }

        private static Uri GenerateCoverUrl(string id)
        {
            var idEnd = id[^1];
            string picEnd;
            if (idEnd == '2')
            {
                picEnd = string.Empty;
            }
            else
            {
                picEnd = (char.GetNumericValue(idEnd) - 1).ToString();
            }

            var url = string.Format(
                "https://www.asianscreens.com/products/400000/portraits/{0}{1}.jpg",
                id.TrimEnd(idEnd),
                picEnd);

            return new Uri(url);
        }

        private async Task<IEnumerable<(string name, string id, Uri cover)>> SearchHelper(string searchName)
        {
            var response = await this.httpClient.GetAsync($"https://www.asianscreens.com/directory/{searchName[0]}.asp").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<(string, string, Uri)>();
            }

            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await this.context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var actressRows = doc.QuerySelectorAll("table[bgcolor='#000000']")
                                 .First(n => n.InnerHtml.Contains("ACTRESS"))
                                 .QuerySelectorAll("tr:not(:last-child)")
                                 .Skip(2);

            return actressRows.Select(row =>
            {
                var name = row.QuerySelector("td:nth-child(1)")?.TextContent;
                var id = row.QuerySelector("a")
                            ?.GetAttribute("href")
                            .TrimStart('/')
                            .Replace(".asp", string.Empty);
                var cover = GenerateCoverUrl(id);
                return (name, id, cover);
            }).Where(actress => actress.name.Split(' ').Contains(searchName, StringComparer.CurrentCultureIgnoreCase) || Regex.Replace(actress.name, @" #\d", string.Empty) == searchName);
        }
    }
}