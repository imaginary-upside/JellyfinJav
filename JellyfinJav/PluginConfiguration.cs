namespace JellyfinJav
{
    using MediaBrowser.Model.Plugins;

    /// <summary>Represents the ordering of an actress' first and last name.</summary>
    public enum ActressNameOrder
    {
        /// <summary>First - Last / English ordering</summary>
        FirstLast,

        /// <summary>Last - First / Japanese ordering</summary>
        LastFirst,
    }

    /// <summary>Represents what to show in a video's display name.</summary>
    public enum VideoDisplayName
    {
        /// <summary>Ex: ABP-001 The Title of the Video</summary>
        CodeTitle,

        /// <summary>Ex: The Title of the Video</summary>
        Title,
    }

    /// <summary>The Plugin's configuration.</summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>Gets or sets the actress' name order.</summary>
        public ActressNameOrder ActressNameOrder { get; set; } = ActressNameOrder.LastFirst;

        /// <summary>Gets or sets the video's display name.</summary>
        public VideoDisplayName VideoDisplayName { get; set; } = VideoDisplayName.Title;

        /// <summary>Gets or sets a value indicating whether to enable actress metadata fetching.</summary>
        public bool EnableActresses { get; set; } = true;
    }
}