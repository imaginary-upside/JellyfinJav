using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;

namespace JellyfinJav {
    public class Plugin : BasePlugin<PluginConfiguration> {
        public override string Name => "Jellyfin JAV";
        public override Guid Id => Guid.Parse("1d5fffc2-1028-4553-9660-bd4966899e44");
        
        public Plugin(IApplicationPaths applicationPaths,
                      IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer) {}
    }
}
