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

    /// <summary>A web scraping client for asianscreens.com.</summary>
    public class AsianscreensClient
    {
        private static readonly Regex IdFromUrl = new Regex(@".*\/(.*)\.asp", RegexOptions.Compiled);
        private readonly HttpClient httpClient = new HttpClient();
        private readonly IBrowsingContext context = BrowsingContext.New();

        /// <summary>Searches for an actress by name.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>A list of all actresses found.</returns>
        public async Task<IEnumerable<ActressResult>> Search(string searchName)
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

        /// <summary>Same as <see cref="Actress" />, but parses and returns the first found actress.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>The first actress found.</returns>
        public async Task<Actress?> SearchFirst(string searchName)
        {
            var result = await this.Search(searchName).ConfigureAwait(false);

            if (!result.Any())
            {
                return null;
            }

            return await this.LoadActress(result.First().Id).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by id.</summary>
        /// <param name="id">The actress' asianscreens.com identifier.</param>
        /// <returns>The parsed actress.</returns>
        public async Task<Actress?> LoadActress(string id)
        {
            return await this.LoadActress(new Uri($"https://www.asianscreens.com/{id}.asp")).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by url.</summary>
        /// <param name="url">The actress' asianscreens.com absolute url.</param>
        /// <returns>The parsed actress.</returns>
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

            if (id is null || name is null)
            {
                return null;
            }

            return new Actress(
                id: id,
                name: name,
                birthdate: birthdate,
                birthplace: birthplace,
                cover: cover);
        }

        private static string? ExtractId(Uri url)
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

        private static string? ExtractCell(IDocument doc, string cellName)
        {
            var cell = doc.QuerySelectorAll("td")
                          ?.FirstOrDefault(n => n.TextContent == cellName)
                          ?.NextElementSibling
                          .TextContent;

            if (cell == "n/a")
            {
                return null;
            }

            return cell;
        }

        private static string? GetCover(IDocument doc)
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

        private async Task<IEnumerable<ActressResult>> SearchHelper(string searchName)
        {
            var response = await this.httpClient.GetAsync($"https://www.asianscreens.com/directory/{searchName[0]}.asp").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<ActressResult>();
            }

            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await this.context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var actressRows = doc.QuerySelectorAll("table[bgcolor='#000000']")
                                 .First(n => n.InnerHtml.Contains("ACTRESS"))
                                 .QuerySelectorAll("tr:not(:last-child)")
                                 .Skip(2);

            if (actressRows is null)
            {
                return Array.Empty<ActressResult>();
            }

            var actresses = new List<ActressResult>();

            foreach (var row in actressRows)
            {
                var name = row.QuerySelector("td:nth-child(1)")?.TextContent;
                var id = row.QuerySelector("a")
                            ?.GetAttribute("href")
                            .TrimStart('/')
                            .Replace(".asp", string.Empty);
                if (name is null || id is null)
                {
                    continue;
                }

                if (!name.Split(' ').Contains(searchName, StringComparer.CurrentCultureIgnoreCase) && Regex.Replace(name, @" #\d", string.Empty) != searchName)
                {
                    continue;
                }

                var cover = GenerateCoverUrl(id);

                actresses.Add(new ActressResult
                {
                    Name = name,
                    Id = id,
                    Cover = cover,
                });
            }

            return actresses;
        }
    }
}