using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Options;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class FileSystemDataStorage : IDataStorage
	{
		private readonly FileSystemDataStorageSettings settings;

		public FileSystemDataStorage(IOptions<FileSystemDataStorageSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public Uri TranslateInternalUri(Uri internalUri)
		{
			const char segmentsSeparator = '/';

			var uriOriginalString = internalUri.OriginalString;
			if (!uriOriginalString.StartsWith(segmentsSeparator.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
			{
				throw new NotSupportedException($"The format of URI is not supported: '{internalUri}'");
			}

			var relativePath = uriOriginalString
				.TrimStart(segmentsSeparator)
				.Replace(segmentsSeparator, Path.DirectorySeparatorChar);

			var fullPath = Path.Combine(settings.Root, relativePath);
			return new Uri(fullPath);
		}
	}
}
