using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using System.Collections.Generic;

namespace JellyfinJav
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Jellyfin JAV";
        public override Guid Id => Guid.Parse("1d5fffc2-1028-4553-9660-bd4966899e44");
        public static Plugin Instance;

        public Plugin(IApplicationPaths applicationPaths,
                      IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.config_page.html", GetType().Namespace)
                }
            };
        }
    }
}
