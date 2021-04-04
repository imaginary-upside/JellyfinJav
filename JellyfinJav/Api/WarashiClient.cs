namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    /// <summary>A web scraping client for warashi-asian-pornstars.fr.</summary>
    public static class WarashiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly IBrowsingContext Context = BrowsingContext.New();

        /// <summary>Searches for an actress by name.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>A list of all actresses found.</returns>
        public static async Task<IEnumerable<ActressResult>> Search(string searchName)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("recherche_critere", "f"),
                new KeyValuePair<string?, string?>("recherche_valeur", searchName),
                new KeyValuePair<string?, string?>("x", "0"),
                new KeyValuePair<string?, string?>("y", "0"),
            });

            var response = await HttpClient.PostAsync("http://warashi-asian-pornstars.fr/en/s-12/search", form).ConfigureAwait(false);
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var actresses = new List<ActressResult>();

            foreach (var row in doc.QuerySelectorAll(".resultat-pornostar"))
            {
                var name = NormalizeName(row.QuerySelector("p")?.TextContent.Split('-')[0].Trim());
                var id = ExtractId(row.QuerySelector("a")?.GetAttribute("href") ?? string.Empty);
                var cover = "http://warashi-asian-pornstars.fr" + row.QuerySelector("img")?.GetAttribute("src");

                if (name is null || id is null)
                {
                    continue;
                }

                actresses.Add(new ActressResult
                {
                    Name = name,
                    Id = id,
                    Cover = new Uri(cover),
                });
            }

            return actresses;
        }

        /// <summary>Same as <see cref="Actress" />, but parses and returns the first found actress.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>The first actress found.</returns>
        public static async Task<Actress?> SearchFirst(string searchName)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("recherche_critere", "f"),
                new KeyValuePair<string?, string?>("recherche_valeur", searchName),
                new KeyValuePair<string?, string?>("x", "0"),
                new KeyValuePair<string?, string?>("y", "0"),
            });

            var response = await HttpClient.PostAsync("http://warashi-asian-pornstars.fr/en/s-12/search", form).ConfigureAwait(false);
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var id = ExtractId(doc.QuerySelector(".correspondance_exacte a")?.GetAttribute("href") ?? string.Empty);
            if (id is null)
            {
                return null;
            }

            return await LoadActress(id).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by id.</summary>
        /// <param name="id">The actress' asianscreens.com identifier.</param>
        /// <returns>The parsed actress.</returns>
        public static async Task<Actress?> LoadActress(string id)
        {
            var parsedId = id.Split('/');
            if (parsedId.Length != 2)
            {
                return null;
            }

            return await LoadActress(new Uri($"http://warashi-asian-pornstars.fr/en/{parsedId[0]}/anything/anything/{parsedId[1]}")).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by url.</summary>
        /// <param name="url">The actress' asianscreens.com absolute url.</param>
        /// <returns>The parsed actress.</returns>
        private static async Task<Actress?> LoadActress(Uri url)
        {
            var response = await HttpClient.GetAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            var id = ExtractId(url.ToString());
            var name = NormalizeName(doc.QuerySelector("#pornostar-profil [itemprop=name]")?.TextContent) ??
                       NormalizeName(doc.QuerySelector("#main h1")?.TextContent);
            var birthdate = ExtractBirthdate(doc);
            var birthplace = doc.QuerySelector("[itemprop=birthPlace]")?.TextContent.Trim();
            var cover = ExtractCover(doc);

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

        private static DateTime? ExtractBirthdate(IDocument doc)
        {
            var bd = doc.QuerySelector("[itemprop=birthDate]")?.GetAttribute("content");
            if (bd != null)
            {
                return DateTime.Parse(bd);
            }

            return null;
        }

        private static string? ExtractId(string url)
        {
            var match = Regex.Match(url, @"\/en\/(.+?)\/.+\/(\d+)$");

            if (!match.Success)
            {
                return null;
            }

            return $"{match.Groups[1].Value}/{match.Groups[2].Value}";
        }

        private static string? NormalizeName(string? name)
        {
            if (name is null)
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()).Trim();
        }

        private static string? ExtractCover(IDocument doc)
        {
            // First try asian-female-pornstar
            var cover = doc.QuerySelector("#pornostar-profil-photos-0 [itemprop=image]")?.GetAttribute("src") ??
                        doc.QuerySelector("#casting-profil-preview [itemprop=image]")?.GetAttribute("src");

            if (cover == null)
            {
                return null;
            }

            return "http://warashi-asian-pornstars.fr" + cover;
        }
    }
}