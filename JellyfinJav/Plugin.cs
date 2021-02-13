namespace JellyfinJav
{
    using System;
    using System.Collections.Generic;
    using MediaBrowser.Common.Configuration;
    using MediaBrowser.Common.Plugins;
    using MediaBrowser.Model.Plugins;
    using MediaBrowser.Model.Serialization;

    /// <summary>JellyfinJav Plugin.</summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>Initializes a new instance of the <see cref="Plugin"/> class.</summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths" />.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer" />.</param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <summary>Gets the current plugin's instance.</summary>
        public static Plugin? Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "Jellyfin JAV";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("1d5fffc2-1028-4553-9660-bd4966899e44");

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.config_page.html", this.GetType().Namespace),
                },
            };
        }
    }
}
