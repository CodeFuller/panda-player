using System;
using System.IO;
using System.Linq;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Options;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class FileSystemStorage : IFileStorage
	{
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

		public void SaveFile(FilePath filePath, Stream content)
		{
			var fullPath = GetFullPath(filePath);
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
			var sourceFilePath = GetFullPath(source);
			var destinationFilePath = GetFullPath(destination);

			fileSystemFacade.MoveFile(sourceFilePath, destinationFilePath);
		}

		public string CheckoutFile(FilePath filePath)
		{
			var fullPath = GetFullPath(filePath);
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
			return Path.Combine(new[] { rootDirectory }.Concat(filePath).ToArray());
		}

		public void MoveFolder(FilePath source, FilePath destination)
		{
			var sourceFolderPath = GetFullPath(source);
			var destinationFolderPath = GetFullPath(destination);

			fileSystemFacade.MoveDirectory(sourceFolderPath, destinationFolderPath);
		}

		public bool FolderIsEmpty(FilePath folderPath)
		{
			var fullPath = GetFullPath(folderPath);
			return fileSystemFacade.DirectoryIsEmpty(fullPath);
		}

		public void DeleteFolder(FilePath folderPath)
		{
			var fullPath = GetFullPath(folderPath);
			fileSystemFacade.DeleteDirectory(fullPath);
		}
	}
}
