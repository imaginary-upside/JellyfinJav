using MediaBrowser.Model.Plugins;

namespace JellyfinJav
{
    public enum ActressNameOrder
    {
        FirstLast,
        LastFirst
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        public ActressNameOrder actressNameOrder { get; set; } = ActressNameOrder.LastFirst;
    }
}