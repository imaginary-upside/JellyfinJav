using MediaBrowser.Model.Plugins;

namespace JellyfinJav
{
    public enum ActressNameOrder
    {
        FirstLast,
        LastFirst
    }

    public enum VideoDisplayName
    {
        CodeTitle,
        Title
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        public ActressNameOrder ActressNameOrder { get; set; } = ActressNameOrder.LastFirst;
        public VideoDisplayName VideoDisplayName { get; set; } = VideoDisplayName.Title;
        public bool EnableActresses { get; set; } = true;
    }
}