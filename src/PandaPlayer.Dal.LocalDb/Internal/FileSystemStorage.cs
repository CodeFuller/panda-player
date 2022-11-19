using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using PandaPlayer.Core.Facades;
using PandaPlayer.Dal.LocalDb.Inconsistencies.StorageInconsistencies;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Dal.LocalDb.Internal
{
	internal class FileSystemStorage : IFileStorage
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string rootDirectory;

		private readonly IReadOnlyCollection<string> excludePaths;

		public FileSystemStorage(IFileSystemFacade fileSystemFacade, IOptions<FileSystemStorageSettings> options)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));

			var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			this.rootDirectory = settings.Root;
			if (String.IsNullOrWhiteSpace(rootDirectory))
			{
				throw new InvalidOperationException("Storage root is not configured");
			}

			this.excludePaths = (settings.ExcludePaths ?? Enumerable.Empty<string>())
				.Select(GetFullPath)
				.ToList();
		}

		public bool FileExists(FilePath filePath)
		{
			var fullPath = GetFullPathInternal(filePath);
			return fileSystemFacade.FileExists(fullPath);
		}

		public void SaveFile(FilePath filePath, Stream content)
		{
			var fullPath = GetFullPathInternal(filePath);
			if (fileSystemFacade.FileExists(fullPath))
			{
				throw new InvalidOperationException($"Destination file '{fullPath}' already exists");
			}

			using (var fileStream = File.Create(fullPath))
			{
				content.Seek(0, SeekOrigin.Begin);
				content.CopyTo(fileStream);
			}

			CommitFile(fullPath);
		}

		public void MoveFile(FilePath source, FilePath destination)
		{
			var sourceFilePath = GetFullPathInternal(source);
			var destinationFilePath = GetFullPathInternal(destination);

			fileSystemFacade.MoveFile(sourceFilePath, destinationFilePath);
		}

		public string CheckoutFile(FilePath filePath)
		{
			var fullPath = GetFullPathInternal(filePath);
			if (!fileSystemFacade.FileExists(fullPath))
			{
				throw new InvalidOperationException($"Storage file '{fullPath}' does not exist");
			}

			fileSystemFacade.ClearReadOnlyAttribute(fullPath);
			return fullPath;
		}

		public void CommitFile(string fileName)
		{
			if (!fileSystemFacade.FileExists(fileName))
			{
				throw new InvalidOperationException($"Storage file '{fileName}' does not exist");
			}

			fileSystemFacade.SetReadOnlyAttribute(fileName);
		}

		public void DeleteFile(FilePath filePath)
		{
			var fullPath = CheckoutFile(filePath);
			fileSystemFacade.DeleteFile(fullPath);
		}

		public string GetFullPath(FilePath filePath)
		{
			return GetFullPathInternal(filePath);
		}

		private string GetFullPathInternal(FilePath filePath)
		{
			return GetFullPath(Path.Combine(filePath.ToArray()));
		}

		public string GetFullPath(string relativePath)
		{
			return Path.Combine(rootDirectory, relativePath);
		}

		public bool FolderExists(FilePath folderPath)
		{
			var fullPath = GetFullPathInternal(folderPath);
			return fileSystemFacade.DirectoryExists(fullPath);
		}

		public bool FolderIsEmpty(FilePath folderPath)
		{
			var fullPath = GetFullPathInternal(folderPath);
			return fileSystemFacade.DirectoryIsEmpty(fullPath);
		}

		public void CreateFolder(FilePath folderPath)
		{
			var fullPath = GetFullPathInternal(folderPath);
			fileSystemFacade.CreateDirectory(fullPath);
		}

		public void MoveFolder(FilePath source, FilePath destination)
		{
			var sourceFolderPath = GetFullPathInternal(source);
			var destinationFolderPath = GetFullPathInternal(destination);

			fileSystemFacade.MoveDirectory(sourceFolderPath, destinationFolderPath);
		}

		public void DeleteFolder(FilePath folderPath)
		{
			var fullPath = GetFullPathInternal(folderPath);
			fileSystemFacade.DeleteDirectory(fullPath);
		}

		public IEnumerable<string> EnumerateFolders()
		{
			return fileSystemFacade
				.EnumerateDirectories(rootDirectory, "*.*", SearchOption.AllDirectories)
				.Where(path => !FullPathMustBeExcluded(path));
		}

		public IEnumerable<string> EnumerateFiles()
		{
			return fileSystemFacade
				.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
				.Where(path => !FullPathMustBeExcluded(path));
		}

		public void CheckFile(FilePath songPath, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			var fullPath = GetFullPathInternal(songPath);

			if (!fileSystemFacade.GetReadOnlyAttribute(fullPath))
			{
				inconsistenciesHandler(new NoFileReadOnlyAttributeInconsistency(fullPath));
			}
		}

		private bool FullPathMustBeExcluded(string fullPath)
		{
			return excludePaths.Any(ep => fullPath.StartsWith(ep, StringComparison.OrdinalIgnoreCase));
		}
	}
}
