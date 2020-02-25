using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.Linq;
using JellyfinJav.Api;

namespace Tests
{
    public class R18ClientTest
    {
        private R18Client client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new R18Client();
        }

        [Test]
        public async Task TestSearchMany()
        {
            var results = await client.Search("abp");

            Assert.AreEqual(30, results.Count());

            foreach (var result in results)
            {
                Assert.AreEqual(11, result.id.Count());
                Assert.AreEqual(7, result.code.Count());
                Assert.Less(60, result.cover.ToString().Length);
            }
        }

        [Test]
        public async Task TestSearchNone()
        {
            var results = await client.Search("noresult");

            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task TestSearchFirst()
        {
            var expected = new Video(
                id: "mvsd00282",
                code: "MVSD-282",
                title: "Rei Mizuna's Three Hole Rape Fan Thanksgiving",
                actresses: new[] { "Rei Mizuna" },
                genres: new[] { "Orgy", "Featured Actress", "Creampie", "Anal Play", "Cum Swallowing", "Digital Mosaic", "Hi-Def" },
                studio: "M's Video Group",
                boxArt: "https://pics.r18.com/digital/video/mvsd00282/mvsd00282pl.jpg",
                cover: "https://pics.r18.com/digital/video/mvsd00282/mvsd00282ps.jpg",
                releaseDate: DateTime.Parse("2015-12-12")
            );

            var result = await client.SearchFirst("MVSD-282");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestSearchFirstNone()
        {
            var result = await client.SearchFirst("noresult");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task TestLoadVideo()
        {
            var expected = new Video(
                id: "118abp00925",
                code: "ABP-925",
                title: "You Can Really Fuck These Girls?! - The Legendary Pink Salon 13 - Get Your Fill Of A Tall Girl With Big Tits!",
                actresses: new[] { "Reina Kashima" },
                genres: new[] { "Tall Girl", "Featured Actress", "Cosplay", "Creampie", "Sex Toys", "Hi-Def" },
                studio: "Prestige",
                boxArt: "https://pics.r18.com/digital/video/118abp00925/118abp00925pl.jpg",
                cover: "https://pics.r18.com/digital/video/118abp00925/118abp00925ps.jpg",
                releaseDate: DateTime.Parse("2019-11-22")
            );

            var result = await client.LoadVideo("118abp00925");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadVideoInvalid()
        {
            var result = await client.LoadVideo("invalid");

            Assert.AreEqual(null, result);
        }
    }
}