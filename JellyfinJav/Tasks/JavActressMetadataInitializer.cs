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
        private readonly IFileSystem fileSystem;

        public JavActressMetadataInitializer(ILibraryManager libraryManager,
                                             IFileSystem fileSystem)
        {
            this.libraryManager = libraryManager;
            this.fileSystem = fileSystem;
        }

        public async Task Run(IProgress<double> progress,
                              CancellationToken cancellationToken)
        {
            progress.Report(0);

            var options = new MetadataRefreshOptions(
                new DirectoryService(fileSystem)
            )
            {
                ImageRefreshMode = MetadataRefreshMode.FullRefresh,
                MetadataRefreshMode = MetadataRefreshMode.FullRefresh
            };

            var query = new InternalPeopleQuery()
            {
                PersonTypes = new string[] { "JAV Actress" }
            };
            var actresses = libraryManager.GetPeopleItems(query);
            for (int i = 0; i < actresses.Count; ++i)
            {
                var actress = actresses[i];

                if (String.IsNullOrEmpty(actress.Overview))
                    await actress.RefreshMetadata(options, cancellationToken);

                progress.Report(i / actresses.Count * 100);
            }

            progress.Report(100);
        }
    }
}