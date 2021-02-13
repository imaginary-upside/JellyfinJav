namespace JellyfinJav.Api
{
    using System;

    /// <summary>Represents an actress search result.</summary>
    public struct ActressResult
    {
        /// <summary>Gets or sets the actress' English name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the actress' website-specific identifier.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the actress' cover image.</summary>
        public Uri? Cover { get; set; }
    }
}