namespace JellyfinJav.Api
{
    using System;

    /// <summary>Represents a video search result.</summary>
    public struct VideoResult
    {
        /// <summary>Gets or sets the video's jav code.</summary>
        public string Code { get; set; }

        /// <summary>Gets or sets the video's website-specific identifier.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the video's cover image.</summary>
        public Uri? Cover { get; set; }
    }
}