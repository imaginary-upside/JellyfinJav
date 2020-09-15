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
        public ActressNameOrder actressNameOrder { get; set; } = ActressNameOrder.LastFirst;
        public VideoDisplayName videoDisplayName { get; set; } = VideoDisplayName.Title;
    }
}