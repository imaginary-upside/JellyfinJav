#pragma warning disable SA1600

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

    public static class WarashiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly IBrowsingContext Context = BrowsingContext.New();

        public static async Task<IEnumerable<(string name, string id, Uri cover)>> Search(string searchName)
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "recherche_critere", "f" },
                { "recherche_valeur", searchName },
                { "x", "0" },
                { "y", "0" },
            });

            var response = await HttpClient.PostAsync("http://warashi-asian-pornstars.fr/en/s-12/search", form).ConfigureAwait(false);
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = await Context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            return doc.QuerySelectorAll(".resultat-pornostar")
                      .Select(n =>
            {
                var name = NormalizeName(n.QuerySelector(".correspondance-lien")?.TextContent);
                var id = ExtractId(n.QuerySelector("a")?.GetAttribute("href"));
                var cover = "http://warashi-asian-pornstars.fr" + n.QuerySelector("img")?.GetAttribute("src");

                return (name, id, new Uri(cover));
            }).Where(n => string.Equals(NormalizeName(searchName), n.name) || string.Equals(NormalizeName(ReverseName(searchName)), n.name));
        }

        public static async Task<Actress?> SearchFirst(string searchName)
        {
            var results = await Search(searchName).ConfigureAwait(false);

            if (!results.Any())
            {
                return null;
            }

            return await LoadActress(results.First().id).ConfigureAwait(false);
        }

        public static async Task<Actress?> LoadActress(string id)
        {
            var parsedId = id.Split('/');
            if (parsedId.Length != 2)
            {
                return null;
            }

            return await LoadActress(new Uri($"http://warashi-asian-pornstars.fr/en/{parsedId[0]}/anything/anything/{parsedId[1]}")).ConfigureAwait(false);
        }

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
            var name = NormalizeName(doc.QuerySelector("#pornostar-profil [itemprop=name]")?.TextContent);
            var birthdate = ExtractBirthdate(doc);
            var birthplace = doc.QuerySelector("[itemprop=birthPlace]")?.TextContent.Trim();
            var cover = ExtractCover(doc);

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

        private static string ExtractId(string url)
        {
            var match = Regex.Match(url, @"\/en\/(.+?)\/.+\/(\d+)$");

            if (!match.Success)
            {
                return null;
            }

            return $"{match.Groups[1].Value}/{match.Groups[2].Value}";
        }

        private static string NormalizeName(string name)
        {
            if (name == null)
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        private static string ReverseName(string name)
        {
            return string.Join(" ", name.Split(' ').Reverse());
        }

        private static string ExtractCover(IDocument doc)
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