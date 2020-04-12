using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using JellyfinJav.Api;

namespace Tests
{
    public class AsianscreensClientTest
    {
        private AsianscreensClient client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new AsianscreensClient();
        }

        [Test]
        public async Task TestSearchLastFirst()
        {
            var results = await client.Search("Sasaki Aki");
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).name);
        }

        [Test]
        public async Task TestSearchFirstLast()
        {
            var results = await client.Search("Aki Sasaki");
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).name);
        }

        [Test]
        public async Task TestSearchMany()
        {
            var results = await client.Search("Ai Nanase");
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Ai Nanase", results.ElementAt(0).name);
            Assert.AreEqual("Ai Nanase #2", results.ElementAt(1).name);
        }

        [Test]
        public async Task TestSearchManyMany()
        {
            var results = await client.Search("Ai");
            Assert.Greater(results.Count(), 100);
        }

        [Test]
        public async Task TestSearchNone()
        {
            var results = await client.Search("test");
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task TestSearchInvalid()
        {
            var results = await client.Search("æŒç”°æ žé‡Œ");
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task TestSearchFirst()
        {
            var result = await client.SearchFirst("Ai Uehara");

            var expected = new Actress(
                id: "ai_uehara2",
                name: "Ai Uehara",
                birthdate: DateTime.Parse("1992-11-22"),
                birthplace: null,
                cover: "http://www.asianscreens.com/products/400000/portraits/ai_uehara.jpg"
            );

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestSearchFirstNone()
        {
            var result = await client.SearchFirst("test");
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestLoadActress()
        {
            var result = await client.LoadActress("koharu_suzuki2");

            var expected = new Actress(
                id: "koharu_suzuki2",
                name: "Koharu Suzuki",
                birthdate: DateTime.Parse("1993-12-1"),
                birthplace: "Kanagawa",
                cover: "http://www.asianscreens.com/products/400000/portraits/koharu_suzuki.jpg"
            );

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadActressMinimalMetadata()
        {
            var result = await client.LoadActress("amika_tsuboi2");

            var expected = new Actress(
                id: "amika_tsuboi2",
                name: "Amika Tsuboi",
                birthdate: null,
                birthplace: null,
                cover: null
            );

            Assert.AreEqual(expected, result);
        }
    }
}