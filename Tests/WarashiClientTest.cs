#pragma warning disable SA1600

namespace Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using NUnit.Framework;

    public class WarashiClientTest
    {
        [Test]
        public async Task TestSearchLastFirst()
        {
            var results = await WarashiClient.Search("Sasaki Aki").ConfigureAwait(false);
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).Name);
            Assert.AreEqual("s-2-0/2714", results.ElementAt(0).Id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/preview/mini/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).Cover);
        }

        [Test]
        public async Task TestSearchFirstLast()
        {
            var results = await WarashiClient.Search("Maria Nagai").ConfigureAwait(false);
            Assert.AreEqual("Maria Nagai", results.ElementAt(0).Name);
            Assert.AreEqual("s-2-0/3743", results.ElementAt(0).Id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/m/a/3743/maria-nagai/preview/mini/wapdb-maria-nagai-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).Cover);
        }

        [Test]
        public async Task TestLoadActress()
        {
            var result = await WarashiClient.LoadActress("s-2-0/2714").ConfigureAwait(false);

            var expected = new Actress(
                id: "s-2-0/2714",
                name: "Aki Sasaki",
                birthdate: DateTime.Parse("December 24, 1979"),
                birthplace: "Japan, Saitama prefecture",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/profil-0/large/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadActressInvalid()
        {
            var result = await WarashiClient.LoadActress("invalid").ConfigureAwait(false);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestSearchFirst()
        {
            var result = await WarashiClient.SearchFirst("Hiyori Yoshioka").ConfigureAwait(false);

            var expected = new Actress(
                id: "s-2-0/3806",
                name: "Hiyori Yoshioka",
                birthdate: DateTime.Parse("August 08, 1999"),
                birthplace: "Japan",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/h/i/3806/hiyori-yoshioka/profil-0/large/wapdb-hiyori-yoshioka-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestSearchFirstFemalePornstar()
        {
            var result = await WarashiClient.SearchFirst("Ruka Aoi").ConfigureAwait(false);

            // Parsing for female-pornstar results isn't done yet.
            var expected = new Actress(
                id: "s-4-1/14028",
                name: "Ruka Aoi - 藍井る加",
                birthdate: null,
                birthplace: null,
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/r/u/786/ruka-aoi/preview/mini/wapdb-ruka-aoi-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }
    }
}