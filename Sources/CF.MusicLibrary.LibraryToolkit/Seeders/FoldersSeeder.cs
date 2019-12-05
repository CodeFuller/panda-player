using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibraryApi.Client.Contracts.Folders;
using MusicLibraryApi.Client.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit.Seeders
{
	public class FoldersSeeder : IFoldersSeeder
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly IFoldersMutation foldersMutation;

		private readonly ILogger<SeedApiDatabaseCommand> logger;

		private readonly FileSystemStorageSettings settings;

		public FoldersSeeder(IFileSystemFacade fileSystemFacade, IFoldersMutation foldersMutation, ILogger<SeedApiDatabaseCommand> logger, IOptions<FileSystemStorageSettings> options)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.foldersMutation = foldersMutation ?? throw new ArgumentNullException(nameof(foldersMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IDictionary<Uri, int>> SeedFolders(DiscLibrary discLibrary, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding folders ...");

			var ignoredUrls = new[] { new Uri("/.sync", UriKind.Relative), };
			var ignoredPaths = ignoredUrls.Select(GetPathForUri).ToList();

			var discPathsCollection = discLibrary.Discs.Select(d => GetPathForUri(d.Uri));
			var discPaths = new HashSet<string>(discPathsCollection);

			// Full directory path -> folder id
			var folders = new Dictionary<string, int>();

			foreach (var directoryPath in fileSystemFacade.EnumerateDirectories(settings.Root, "*.*", SearchOption.AllDirectories).OrderBy(p => p))
			{
				if (ignoredPaths.Any(s => directoryPath.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
				{
					logger.LogDebug("Skipping directory {DirectoryPath} ...", directoryPath);
					continue;
				}

				if (discPaths.Contains(directoryPath))
				{
					logger.LogDebug("Skipping disc directory {DirectoryPath} ...", directoryPath);
					continue;
				}

				logger.LogInformation("Creating folder {DirectoryPath} ...", directoryPath);

				var folderName = Path.GetFileName(directoryPath);
				var parentDirectoryPath = Directory.GetParent(directoryPath).FullName;

				int? parentFolderId = null;
				if (!folders.TryGetValue(parentDirectoryPath, out var loadedParentFolderId))
				{
					if (!String.Equals(parentDirectoryPath, settings.Root, StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidOperationException($"The id of parent folder is unknown - {parentDirectoryPath}");
					}
				}
				else
				{
					parentFolderId = loadedParentFolderId;
				}

				var folderData = new InputFolderData(folderName, parentFolderId);
				var createdFolderId = await foldersMutation.CreateFolder(folderData, cancellationToken);

				folders.Add(directoryPath, createdFolderId);
			}

			logger.LogInformation("Seeded {FoldersNumber} folders", folders.Count);

			return folders.Select(p => new KeyValuePair<Uri, int>(GetUriForPath(p.Key), p.Value)).ToDictionary(p => p.Key, p => p.Value);
		}

		private Uri GetUriForPath(string path)
		{
			if (!path.StartsWith(settings.Root, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"The path {path} is not within the library root {settings.Root}");
			}

			var relativePath = path.Substring(settings.Root.Length).Replace('\\', '/');
			return new Uri(relativePath, UriKind.Relative);
		}

		private string GetPathForUri(Uri uri)
		{
			var segments = new List<string>
			{
				settings.Root,
			};
			segments.AddRange(uri.SegmentsEx());

			return Path.Combine(segments.ToArray());
		}
	}
}
