namespace JellyfinJav.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>A struct representing a japanese adult video (JAV).</summary>
    public readonly struct Video
    {
        /// <summary>The website-specific identifier.</summary>
        public readonly string Id;

        /// <summary>The jav code. Ex: ABP-001.</summary>
        public readonly string Code;

        /// <summary>The video's English title.</summary>
        public readonly string Title;

        /// <summary>A list of every actress in the video.</summary>
        public readonly IEnumerable<string> Actresses;

        /// <summary>A list of the video's genres.</summary>
        public readonly IEnumerable<string> Genres;

        /// <summary>The studio which released the video.</summary>
        public readonly string? Studio;

        /// <summary>An absolute url to the boxart.</summary>
        public readonly string? BoxArt;

        /// <summary>An absolute url to the cover image.</summary>
        public readonly string? Cover;

        /// <summary>The date which the video was released.</summary>
        public readonly DateTime? ReleaseDate;

        /// <summary>Initializes a new instance of the <see cref="Video" /> struct.</summary>
        /// <param name="id">The website-specific identifier.</param>
        /// <param name="code">The jav code. Ex: ABP-001.</param>
        /// <param name="title">The English title.</param>
        /// <param name="actresses">Every actress in the video.</param>
        /// <param name="genres">The video's genres.</param>
        /// <param name="studio">The studio which released the video.</param>
        /// <param name="boxArt">An absolute url to the boxart.</param>
        /// <param name="cover">An absolute url to the cover image.</param>
        /// <param name="releaseDate">The date which the video was released.</param>
        public Video(
            string id,
            string code,
            string title,
            IEnumerable<string> actresses,
            IEnumerable<string> genres,
            string? studio,
            string? boxArt,
            string? cover,
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

        /// <summary>Checks if two Video objects are equal.</summary>
        /// <param name="v1">The first video.</param>
        /// <param name="v2">The second video.</param>
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

        /// <summary>Checks if two Video objects are not equal.</summary>
        /// <param name="v1">The first video.</param>
        /// <param name="v2">The second video.</param>
        public static bool operator !=(Video v1, Video v2)
        {
            return !(v1 == v2);
        }

        /// <summary>Prints out each of the video's public variables.</summary>
        /// <returns>All the public variables.</returns>
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

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                   this.Code.GetHashCode() ^
                   this.Title.GetHashCode() ^
                   this.Actresses.GetHashCode() ^
                   this.Genres.GetHashCode() ^
                   this.Studio?.GetHashCode() ?? 0 ^
                   this.BoxArt?.GetHashCode() ?? 0 ^
                   this.Cover?.GetHashCode() ?? 0 ^
                   this.ReleaseDate.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Video o && this == o;
        }
    }
}