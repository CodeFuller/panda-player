﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Options;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class FileSystemStorage : IFileStorage, IUriTranslator
	{
		private const char SegmentsSeparator = '/';

		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string rootDirectory;

		public FileSystemStorage(IFileSystemFacade fileSystemFacade, IOptions<FileSystemStorageSettings> options)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));

			this.rootDirectory = options?.Value?.Root;
			if (String.IsNullOrWhiteSpace(rootDirectory))
			{
				throw new InvalidOperationException("Storage root is not configured");
			}
		}

		public Uri GetExternalUri(Uri internalUri)
		{
			var filePath = GetFilePathForInternalUri(internalUri);
			return new Uri(filePath);
		}

		public Uri GetInternalUri(Uri externalUri)
		{
			var filePath = GetFilePathForExternalUri(externalUri);
			var relativePath = Path.GetRelativePath(rootDirectory, filePath);

			return GetInternalUriForRelativePath(relativePath);
		}

		public Uri ReplaceSegmentInExternalUri(Uri externalUri, string newValue, int segmentIndex)
		{
			var internalUri = GetInternalUri(externalUri);
			var segments = internalUri.OriginalString.Split(SegmentsSeparator).Skip(1).ToArray();
			var index = segmentIndex > 0 ? segmentIndex : segments.Length + segmentIndex;

			segments[index] = newValue;
			var newInternalUri = BuildInternalUriFromSegments(segments);

			return GetExternalUri(newInternalUri);
		}

		public void MoveFile(Uri currentFileUri, Uri newFileUri)
		{
			var sourceFilePath = GetFilePathForExternalUri(currentFileUri);
			var destinationFilePath = GetFilePathForExternalUri(newFileUri);

			fileSystemFacade.MoveFile(sourceFilePath, destinationFilePath);
		}

		public string CheckoutFile(Uri fileUri)
		{
			var filePath = GetFilePathForExternalUri(fileUri);
			if (!fileSystemFacade.FileExists(filePath))
			{
				throw new InvalidOperationException($"Storage file '{filePath}' does not exist");
			}

			fileSystemFacade.ClearReadOnlyAttribute(filePath);
			return filePath;
		}

		public void CommitFile(string fileName)
		{
			if (!fileSystemFacade.FileExists(fileName))
			{
				throw new InvalidOperationException($"Storage file '{fileName}' does not exist");
			}

			fileSystemFacade.SetReadOnlyAttribute(fileName);
		}

		public void DeleteFile(Uri fileUri)
		{
			var filePath = CheckoutFile(fileUri);
			fileSystemFacade.DeleteFile(filePath);
		}

		private string GetFilePathForInternalUri(Uri internalUri)
		{
			var uriOriginalString = internalUri.OriginalString;
			if (!uriOriginalString.StartsWith(SegmentsSeparator.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
			{
				throw new NotSupportedException($"The format of URI is not supported: '{internalUri}'");
			}

			var relativePath = uriOriginalString
				.TrimStart(SegmentsSeparator)
				.Replace(SegmentsSeparator, Path.DirectorySeparatorChar);

			return Path.Combine(rootDirectory, relativePath);
		}

		private static string GetFilePathForExternalUri(Uri externalUri)
		{
			if (!externalUri.IsFile)
			{
				throw new InvalidOperationException($"File URI expected: {externalUri}");
			}

			return externalUri.LocalPath;
		}

		private static Uri GetInternalUriForRelativePath(string relativePath)
		{
			var segments = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			return BuildInternalUriFromSegments(segments);
		}

		private static Uri BuildInternalUriFromSegments(IEnumerable<string> segments)
		{
			return new Uri($"{SegmentsSeparator}{String.Join(SegmentsSeparator, segments)}", UriKind.Relative);
		}
	}
}
