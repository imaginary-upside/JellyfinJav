#pragma warning disable SA1600

namespace Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using NUnit.Framework;

    public class AsianscreensClientTest
    {
        private AsianscreensClient client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            this.client = new AsianscreensClient();
        }

        [Test]
        public async Task TestSearchLastFirst()
        {
            var results = await this.client.Search("Sasaki Aki").ConfigureAwait(false);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).Name);
        }

        [Test]
        public async Task TestSearchFirstLast()
        {
            var results = await this.client.Search("Aki Sasaki").ConfigureAwait(false);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).Name);
        }

        [Test]
        public async Task TestSearchMany()
        {
            var results = await this.client.Search("Ai Nanase").ConfigureAwait(false);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Ai Nanase", results.ElementAt(0).Name);
            Assert.AreEqual("Ai Nanase #2", results.ElementAt(1).Name);
        }

        [Test]
        public async Task TestSearchManyMany()
        {
            var results = await this.client.Search("Ai").ConfigureAwait(false);
            Assert.Greater(results.Count(), 100);
        }

        [Test]
        public async Task TestSearchNone()
        {
            var results = await this.client.Search("test").ConfigureAwait(false);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task TestSearchInvalid()
        {
            var results = await this.client.Search("æŒç”°æ žé‡Œ").ConfigureAwait(false);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task TestSearchFirst()
        {
            var result = await this.client.SearchFirst("Ai Uehara").ConfigureAwait(false);

            var expected = new Actress(
                id: "ai_uehara2",
                name: "Ai Uehara",
                birthdate: DateTime.Parse("1992-11-22"),
                birthplace: null,
                cover: "https://www.asianscreens.com/products/400000/portraits/ai_uehara.jpg");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestSearchFirstNone()
        {
            var result = await this.client.SearchFirst("test").ConfigureAwait(false);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestLoadActress()
        {
            var result = await this.client.LoadActress("koharu_suzuki2").ConfigureAwait(false);

            var expected = new Actress(
                id: "koharu_suzuki2",
                name: "Koharu Suzuki",
                birthdate: DateTime.Parse("1993-12-1"),
                birthplace: "Kanagawa",
                cover: "https://www.asianscreens.com/products/400000/portraits/koharu_suzuki.jpg");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadActressMinimalMetadata()
        {
            var result = await this.client.LoadActress("amika_tsuboi2").ConfigureAwait(false);

            var expected = new Actress(
                id: "amika_tsuboi2",
                name: "Amika Tsuboi",
                birthdate: null,
                birthplace: null,
                cover: null);

            Assert.AreEqual(expected, result);
        }
    }
}