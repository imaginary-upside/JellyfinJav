using AngleSharp;
using AngleSharp.Dom;
using CloudflareSolverRe;
using System;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace JellyfinJav.Api
{
    /// <summary>A web scraping client for javlibrary.com</summary>
    public class JavlibraryClient
    {
        private readonly HttpClient httpClient;
        private readonly IBrowsingContext context;

        public JavlibraryClient()
        {
            var handler = new ClearanceHandler();
            httpClient = new HttpClient(handler);
            context = BrowsingContext.New();
        }

        /// <summary>Searches by the specified identifier.</summary>
        /// <param name="identifier">The identifier to search for.</param>
        /// <returns>An array of tuples representing each video returned during the search.</returns>
        /// <example>
        /// <code>
        /// var client = new Javlibrary.Client();
        /// var results = client.Search("abp");
        /// results.Count // 20
        /// results[10].code // ABP-011
        /// results[10].url // https://http://www.javlibrary.com/en/?v=javlijdqrm
        /// </code>
        /// </example>
        public async Task<IEnumerable<(string code, Uri url)>> Search(string identifier)
        {
            var doc = await LoadPage("https://www.javlibrary.com/en/vl_searchbyid.php?keyword=" + identifier);

            // if only one result was found, and so we were taken directly to the video page.
            if (doc.QuerySelector("#video_id") != null)
            {
                var resultCode = doc.QuerySelector("#video_id .text")?.TextContent;
                var url = new Uri("https://www.javlibrary.com" + doc.QuerySelector("#video_title a")?.GetAttribute("href"));
                return new[] { (resultCode, url) };
            }

            return doc.QuerySelectorAll(".video").Select(n =>
            {
                var code = n.QuerySelector(".id").TextContent;
                var url = new Uri("https://www.javlibrary.com/en/" + n.QuerySelector("a")?.GetAttribute("href"));
                return (code, url);
            });
        }

        /// <summary>Searches for a specific JAV code, and returns the first result.</summary>
        /// <param name="code">The JAV to search for.</param>
        /// <returns>The first result of the search, or null if nothing was found.</returns>
        /// <example>
        /// <code>
        /// var client = new Javlibrary.Client();
        /// var result = client.SearchFirst("ABP-020");
        /// result.Id // javlijazsu
        /// result.Title // Fan Fan PRESTIGE Large Thanksgiving Soil And Shiro To Spree Yamakawa Blue Sky Meets Escalate! Basutsua ~
        /// </code>
        /// </example>
        public async Task<Video?> SearchFirst(string code)
        {
            var doc = await LoadPage("http://www.javlibrary.com/en/vl_searchbyid.php?keyword=" + code);

            if (doc.QuerySelector("p em")?.TextContent == "Search returned no result." ||
                doc.QuerySelector("#badalert td")?.TextContent == "The search term you entered is invalid. Please try a different term.")
                return null;

            // if only one result was found, and so we were taken directly to the video page.
            if (doc.QuerySelector("#video_id") != null)
                return ParseVideoPage(doc);

            return await LoadVideo(new Uri("https://www.javlibrary.com/en/" + doc.QuerySelector(".video a")?.GetAttribute("href")));
        }

        /// <summary>Loads a specific JAV by id</summary>
        /// <param name="id">The JavLibrary spcific JAV identifier</param>
        /// <returns>The parsed video, or null if no video with <c>id</c> exists.</returns>
        /// <example>
        /// <code>
        /// var client = new Javlibrary.Client();
        /// var result = client.LoadVideo("javlijazsu");
        /// result.Id // javlijazsu
        /// result.Title // Fan Fan PRESTIGE Large Thanksgiving Soil And Shiro To Spree Yamakawa Blue Sky Meets Escalate! Basutsua ~
        /// </code>
        /// </example>
        public async Task<Video> LoadVideo(string id)
        {
            return await LoadVideo(new Uri("https://www.javlibrary.com/en/?v=" + id));
        }

        /// <summary>Loads a specific JAV by url</summary>
        /// <param name="url">The JAV url</param>
        /// <returns>The parsed video, or null if no video at <c>url</c> exists.</returns>
        /// <example>
        /// <code>
        /// var client = new Javlibrary.Client();
        /// var result = client.LoadVideo(new Url("http://www.javlibrary.com/en/?v=javlijazsu"));
        /// result.Id // javlijazsu
        /// result.Title // Fan Fan PRESTIGE Large Thanksgiving Soil And Shiro To Spree Yamakawa Blue Sky Meets Escalate! Basutsua ~
        /// </code>
        /// </example>
        public async Task<Video> LoadVideo(Uri url)
        {
            var response = await httpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var doc = await context.OpenAsync(req => req.Content(html));
            return ParseVideoPage(doc);
        }

        private async Task<IDocument> LoadPage(string url)
        {
            var response = await httpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return await context.OpenAsync(req => req.Content(html));
        }

        private Video ParseVideoPage(IDocument doc)
        {
            var id = HttpUtility.ParseQueryString(
                new Uri("https://www.javlibrary.com" + doc.QuerySelector("#video_title a")?.GetAttribute("href")).Query
            )["v"];
            var code = doc.QuerySelector("#video_id .text")?.TextContent;
            var actresses = doc.QuerySelectorAll(".star a").Select(n => n.TextContent);
            var title = doc.QuerySelector("#video_title a")?
                           .TextContent
                           .Replace(code, "")
                           .TrimStart(' ')
                           .Trim(actresses.FirstOrDefault())
                           .Trim(ReverseName(actresses.FirstOrDefault()))
                           .Trim();
            var genres = doc.QuerySelectorAll(".genre a").Select(n => n.TextContent);
            var studio = doc.QuerySelector("#video_maker a")?.TextContent;
            var boxArt = doc.QuerySelector("#video_jacket_img")?.GetAttribute("src")?.Insert(0, "https:");
            var cover = boxArt.Replace("pl.jpg", "ps.jpg");

            return new Api.Video(
                id: id,
                code: code,
                title: title,
                actresses: actresses,
                genres: genres,
                studio: studio,
                boxArt: boxArt,
                cover: cover,
                releaseDate: null // TODO
            );
        }

        private static string ReverseName(in string name)
        {
            if (name == null)
                return null;

            return String.Join(" ", name.Split(' ').Reverse());
        }
    }
}