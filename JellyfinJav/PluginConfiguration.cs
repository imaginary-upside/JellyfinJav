#pragma warning disable SA1600, SA1602, CS1591

namespace JellyfinJav
{
    using MediaBrowser.Model.Plugins;

    public enum ActressNameOrder
    {
        FirstLast,
        LastFirst,
    }

    public enum VideoDisplayName
    {
        CodeTitle,
        Title,
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        public ActressNameOrder ActressNameOrder { get; set; } = ActressNameOrder.LastFirst;

        public VideoDisplayName VideoDisplayName { get; set; } = VideoDisplayName.Title;

        public bool EnableActresses { get; set; } = true;
    }
}