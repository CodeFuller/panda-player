using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.Library
{
	public class FileSystemStorage : IFileStorage
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string storageRootDirectory;

		public FileSystemStorage(IFileSystemFacade fileSystemFacade, IOptions<FileSystemStorageSettings> options)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.storageRootDirectory = options?.Value?.Root ?? throw new ArgumentNullException(nameof(options));
		}

		public void StoreFile(string sourceFileName, Uri fileUri)
		{
			var targetFileName = GetPathForUri(fileUri);
			fileSystemFacade.CreateDirectory(Path.GetDirectoryName(targetFileName));
			fileSystemFacade.CopyFile(sourceFileName, targetFileName);
			fileSystemFacade.SetReadOnlyAttribute(targetFileName);
		}

		public string GetFile(Uri fileUri)
		{
			var fileName = GetPathForUri(fileUri);
			if (!fileSystemFacade.FileExists(fileName))
			{
				throw new InvalidOperationException($"Storage file '{fileName}' does not exist");
			}

			return fileName;
		}

		public string GetFileForWriting(Uri fileUri)
		{
			var fileName = GetFile(fileUri);
			fileSystemFacade.ClearReadOnlyAttribute(fileName);
			return fileName;
		}

		public void UpdateFileContent(string sourceFileName, Uri fileUri)
		{
			var targetFileName = GetPathForUri(fileUri);
			if (!fileSystemFacade.FileExists(targetFileName))
			{
				throw new InvalidOperationException($"Storage file '{targetFileName}' does not exist");
			}

			if (String.Equals(sourceFileName, targetFileName, StringComparison.OrdinalIgnoreCase))
			{
				fileSystemFacade.SetReadOnlyAttribute(targetFileName);
			}
			else
			{
				DeleteFile(targetFileName);
				StoreFile(sourceFileName, fileUri);
			}
		}

		public void MoveFile(Uri currFileUri, Uri newFileUri)
		{
			fileSystemFacade.MoveFile(GetPathForUri(currFileUri), GetPathForUri(newFileUri));
		}

		public void MoveDirectory(Uri currDirectoryUri, Uri newDirectoryUri)
		{
			fileSystemFacade.MoveDirectory(GetPathForUri(currDirectoryUri), GetPathForUri(newDirectoryUri));
		}

		public void DeleteFile(Uri fileUri)
		{
			string fileName = GetPathForUri(fileUri);
			DeleteFile(fileName);

			// Deleting current and parent directories within storage that became empty.
			foreach (var currDirectoryPath in GetParentDirectoriesWithinStorage(fileSystemFacade.GetParentDirectory(fileName))
												.TakeWhile(dir => fileSystemFacade.DirectoryIsEmpty(dir)))
			{
				fileSystemFacade.DeleteDirectory(currDirectoryPath);
			}
		}

		private void DeleteFile(string fileName)
		{
			fileSystemFacade.ClearReadOnlyAttribute(fileName);
			fileSystemFacade.DeleteFile(fileName);
		}

		private IEnumerable<string> GetParentDirectoriesWithinStorage(string startDirectoryPath)
		{
			string currDirectoryPath = startDirectoryPath;
			do
			{
				yield return currDirectoryPath;
				currDirectoryPath = fileSystemFacade.GetParentDirectory(currDirectoryPath);
			}
			while (currDirectoryPath != null &&
						!String.Equals(fileSystemFacade.GetFullPath(currDirectoryPath), fileSystemFacade.GetFullPath(storageRootDirectory), StringComparison.OrdinalIgnoreCase));
		}

		public void CheckDataConsistency(IEnumerable<Uri> expectedFileUris, IEnumerable<Uri> ignoreList, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			var expectedFileUrisList = expectedFileUris.ToList();
			var expectedFileNames = expectedFileUrisList.Select(GetPathForUri).ToList();
			var ignoredPaths = ignoreList.Select(GetPathForUri).ToList();

			foreach (var expectedFileUri in expectedFileUrisList)
			{
				string fileName = GetPathForUri(expectedFileUri);
				if (!fileSystemFacade.FileExists(fileName))
				{
					registrator.RegisterMissingStorageData(expectedFileUri);
				}
				else if (!fileSystemFacade.GetReadOnlyAttribute(fileName))
				{
					registrator.RegisterErrorInStorageData($"Storage file has no read-only attribute set: {fileName}");
					if (fixFoundIssues)
					{
						fileSystemFacade.SetReadOnlyAttribute(fileName);
						registrator.RegisterFixOfErrorInStorageData($"Read-only attribute has been set for '{fileName}'");
					}
				}
			}

			foreach (var storageFileName in fileSystemFacade.EnumerateFiles(storageRootDirectory, "*.*", SearchOption.AllDirectories))
			{
				if (!ignoredPaths.Any(s => storageFileName.StartsWith(s, StringComparison.OrdinalIgnoreCase)) &&
					!expectedFileNames.Contains(storageFileName))
				{
					registrator.RegisterUnexpectedStorageData(storageFileName, "file");
				}
			}

			foreach (string directory in fileSystemFacade.EnumerateDirectories(storageRootDirectory, "*.*", SearchOption.AllDirectories))
			{
				if (!ignoredPaths.Any(s => directory.StartsWith(s, StringComparison.OrdinalIgnoreCase)) &&
					!expectedFileNames.Any(fileName => fileName.StartsWith(directory, StringComparison.OrdinalIgnoreCase)))
				{
					registrator.RegisterUnexpectedStorageData(directory, "directory");
				}
			}
		}

		private string GetPathForUri(Uri uri)
		{
			List<string> segments = new List<string>
			{
				storageRootDirectory,
			};
			segments.AddRange(uri.SegmentsEx());

			return Path.Combine(segments.ToArray());
		}
	}
}
