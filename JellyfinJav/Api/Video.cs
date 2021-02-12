#pragma warning disable SA1600

namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public Video(
            string id,
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

        public override string ToString()
        {
            return $"Id: {this.Id}\n" +
                   $"Code: {this.Code}\n" +
                   $"Title: {this.Title}\n" +
                   $"Actresses: {string.Join(", ", this.Actresses)}\n" +
                   $"Genres: {string.Join(", ", this.Genres)}\n" +
                   $"Studio: {this.Studio}\n" +
                   $"BoxArt: {this.BoxArt}\n" +
                   $"Cover: {this.Cover}\n" +
                   $"ReleaseDate: {this.ReleaseDate}\n";
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                   this.Code.GetHashCode() ^
                   this.Title.GetHashCode() ^
                   this.Actresses.GetHashCode() ^
                   this.Genres.GetHashCode() ^
                   this.Studio.GetHashCode() ^
                   this.BoxArt.GetHashCode() ^
                   this.Cover.GetHashCode() ^
                   this.ReleaseDate.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Video o && this == o;
        }
    }
}