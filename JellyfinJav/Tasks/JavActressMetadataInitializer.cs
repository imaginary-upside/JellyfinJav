using System.Threading;
using System.Threading.Tasks;
using System;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;

namespace JellyfinJav.Tasks
{
    public class JavActressMetadataInitializer : ILibraryPostScanTask
    {
        private readonly ILibraryManager libraryManager;
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;

        public JavActressMetadataInitializer(ILibraryManager libraryManager,
                                             ILogger logger,
                                             IFileSystem fileSystem)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
            this.fileSystem = fileSystem;
        }

        public async Task Run(IProgress<double> _progress,
                              CancellationToken cancellationToken)
        {
            var options = new MetadataRefreshOptions(
                new DirectoryService(logger, fileSystem)
            )
            {
                ImageRefreshMode = MetadataRefreshMode.FullRefresh,
                MetadataRefreshMode = MetadataRefreshMode.FullRefresh
            };

            var actresses = libraryManager.GetPeopleItems(new InternalPeopleQuery()
            {
                PersonTypes = new string[] { "JAV Actress" }
            });
            foreach (var actress in actresses)
            {
                if (actress.DateLastRefreshed == default(DateTime))
                {
                    await actress.RefreshMetadata(options, cancellationToken);
                }
            }
        }
    }
}