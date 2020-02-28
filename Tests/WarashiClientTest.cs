using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using JellyfinJav.Api;

namespace Tests
{
    public class WarashiClientTest
    {
        private WarashiClient client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new WarashiClient();
        }

        [Test]
        public async Task TestSearchLastFirst()
        {
            var results = await client.Search("Sasaki Aki");
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).name);
            Assert.AreEqual("2714", results.ElementAt(0).id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/preview/mini/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).cover
            );
        }

        [Test]
        public async Task TestSearchFirstLast()
        {
            var results = await client.Search("Maria Nagai");
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Maria Nagai", results.ElementAt(0).name);
            Assert.AreEqual("3743", results.ElementAt(0).id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/m/a/3743/maria-nagai/preview/mini/wapdb-maria-nagai-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).cover
            );
        }

        [Test]
        public async Task TestLoadActress()
        {
            var result = await client.LoadActress("2714");

            var expected = new Actress(
                id: "2714",
                name: "Aki Sasaki",
                birthdate: DateTime.Parse("December 24, 1979"),
                birthplace: "Japan, Saitama prefecture",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/profil-0/large/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"
            );

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadActressInvalid()
        {
            var result = await client.LoadActress("invalid");
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestSearchFirst()
        {
            var result = await client.SearchFirst("Hiyori Yoshioka");

            var expected = new Actress(
                id: "3806",
                name: "Hiyori Yoshioka",
                birthdate: DateTime.Parse("August 08, 1999"),
                birthplace: "Japan",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/h/i/3806/hiyori-yoshioka/profil-0/large/wapdb-hiyori-yoshioka-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"
            );

            Assert.AreEqual(expected, result);
        }
    }
}