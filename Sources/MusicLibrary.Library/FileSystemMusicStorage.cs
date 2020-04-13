using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Library
{
	public class FileSystemMusicStorage : IMusicLibraryStorage
	{
		private readonly IFileStorage fileStorage;

		public FileSystemMusicStorage(IFileStorage fileStorage)
		{
			this.fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
		}

		public async Task StoreSong(string sourceSongFileName, Song song)
		{
			await Task.Run(() => fileStorage.StoreFile(sourceSongFileName, song.Uri));
		}

		public Task<string> GetSongFile(Song song)
		{
			return Task.FromResult(fileStorage.GetFile(song.Uri));
		}

		public Task<string> GetSongFileForWriting(Song song)
		{
			return Task.FromResult(fileStorage.GetFileForWriting(song.Uri));
		}

		public async Task UpdateSongContent(string sourceSongFileName, Song song)
		{
			await Task.Run(() => fileStorage.UpdateFileContent(sourceSongFileName, song.Uri));
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await Task.Run(() => fileStorage.MoveFile(song.Uri, newSongUri));
		}

		public async Task DeleteSong(Song song)
		{
			await Task.Run(() => fileStorage.DeleteFile(song.Uri));
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await Task.Run(() => fileStorage.MoveDirectory(disc.Uri, newDiscUri));
		}

		public async Task StoreDiscImage(string sourceImageFileName, DiscImage discImage)
		{
			await Task.Run(() => fileStorage.StoreFile(sourceImageFileName, discImage.Uri));
		}

		public async Task<string> GetDiscImageFile(DiscImage discImage)
		{
			return await Task.FromResult(fileStorage.GetFile(discImage.Uri));
		}

		public async Task DeleteDiscImage(DiscImage discImage)
		{
			await Task.Run(() => fileStorage.DeleteFile(discImage.Uri));
		}

		public async Task CheckDataConsistency(IEnumerable<Uri> expectedItemUris, IUriCheckScope checkScope, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			await Task.Run(() => fileStorage.CheckDataConsistency(expectedItemUris, new[] { new Uri("/.sync", UriKind.Relative) }, checkScope, registrator, fixFoundIssues));
		}
	}
}
