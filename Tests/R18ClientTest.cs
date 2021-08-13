#pragma warning disable SA1600

namespace Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JellyfinJav.Api;
    using NUnit.Framework;

    public class R18ClientTest
    {
        [Test]
        public async Task TestSearchMany()
        {
            var results = await R18Client.Search("sora").ConfigureAwait(false);

            Assert.AreEqual(30, results.Count());
        }

        [Test]
        public async Task TestSearchNone()
        {
            var results = await R18Client.Search("noresult").ConfigureAwait(false);

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
                releaseDate: DateTime.Parse("2015-12-12 10:00:21"));

            var result = await R18Client.SearchFirst("MVSD-282").ConfigureAwait(false);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestSearchFirstNone()
        {
            var result = await R18Client.SearchFirst("noresult").ConfigureAwait(false);

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task TestLoadVideo()
        {
            var expected = new Video(
                id: "ssni00643",
                code: "SSNI-643",
                title: "A Woman With Divine Titties Lala Anzai Her Adult Video Debut Miraculous Huge Tits A 7-Tit-Jamming Special",
                actresses: new[] { "Rara Anzai" },
                genres: new[] { "Big Tits", "Big Tits Lover", "Featured Actress", "Titty Fuck", "Debut", "Minimal Mosaic", "Hi-Def" },
                studio: "S1 NO.1 STYLE",
                boxArt: "https://pics.r18.com/digital/video/ssni00643/ssni00643pl.jpg",
                cover: "https://pics.r18.com/digital/video/ssni00643/ssni00643ps.jpg",
                releaseDate: DateTime.Parse("2019-11-30 10:00:59"));

            var result = await R18Client.LoadVideo("ssni00643").ConfigureAwait(false);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadVideoNoActress()
        {
            var expected = new Video(
                id: "njvr00023",
                code: "NJVR-023",
                title: "[VR] The Horn Dogs Chose My Apartment To Be Their Fuck Pad. My Friend Was A Nampa Artist And He Brought Over Tsubasa-chan For Some Lotion Lathered Slick And Slippery Fucking",
                actresses: Array.Empty<string>(),
                genres: new[] { "Beautiful Girl", "Big Tits", "Threesome / Foursome", "Lotion", "POV", "VR Exclusive" },
                studio: "Nanpa JAPAN",
                boxArt: "https://pics.r18.com/digital/video/njvr00023/njvr00023pl.jpg",
                cover: "https://pics.r18.com/digital/video/njvr00023/njvr00023ps.jpg",
                releaseDate: DateTime.Parse("2019-07-26 10:00:31"));

            var result = await R18Client.LoadVideo("njvr00023").ConfigureAwait(false);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task TestLoadVideoInvalid()
        {
            var result = await R18Client.LoadVideo("invalid").ConfigureAwait(false);

            Assert.AreEqual(null, result);
        }
    }
}