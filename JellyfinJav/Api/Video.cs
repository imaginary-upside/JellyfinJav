using System;
using System.Collections.Generic;
using System.Linq;

namespace JellyfinJav.Api
{
    public readonly struct Video
    {
        public readonly string Id;
        public readonly string Code;
        public readonly string Title;
        public readonly IEnumerable<string> Actresses;
        public readonly IEnumerable<string> Genres;
        public readonly string Studio;
        public readonly string BoxArt;
        public readonly string Cover;
        public readonly DateTime? ReleaseDate;

        public Video(string id,
                     string code,
                     string title,
                     IEnumerable<string> actresses,
                     IEnumerable<string> genres,
                     string studio,
                     string boxArt,
                     string cover,
                     DateTime? releaseDate)
        {
            this.Id = id;
            this.Code = code;
            this.Title = title;
            this.Actresses = actresses;
            this.Genres = genres;
            this.Studio = studio;
            this.BoxArt = boxArt;
            this.Cover = cover;
            this.ReleaseDate = releaseDate;
        }

        public override string ToString()
        {
            return $"Id: {Id}\n" +
                   $"Code: {Code}\n" +
                   $"Title: {Title}\n" +
                   $"Actresses: {string.Join(", ", Actresses)}\n" +
                   $"Genres: {string.Join(", ", Genres)}\n" +
                   $"Studio: {Studio}\n" +
                   $"BoxArt: {BoxArt}\n" +
                   $"Cover: {Cover}\n" +
                   $"ReleaseDate: {ReleaseDate}\n";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^
                   Code.GetHashCode() ^
                   Title.GetHashCode() ^
                   Actresses.GetHashCode() ^
                   Genres.GetHashCode() ^
                   Studio.GetHashCode() ^
                   BoxArt.GetHashCode() ^
                   Cover.GetHashCode() ^
                   ReleaseDate.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Video o && this == o;
        }

        public static bool operator ==(Video v1, Video v2)
        {
            return v1.Id == v2.Id &&
                   v1.Code == v2.Code &&
                   v1.Title == v2.Title &&
                   v1.Actresses.SequenceEqual(v2.Actresses) &&
                   v1.Genres.SequenceEqual(v2.Genres) &&
                   v1.Studio == v2.Studio &&
                   v1.BoxArt == v2.BoxArt &&
                   v1.Cover == v2.Cover &&
                   v1.ReleaseDate == v2.ReleaseDate;
        }

        public static bool operator !=(Video v1, Video v2)
        {
            return !(v1 == v2);
        }
    }
}