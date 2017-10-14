using System;
using System.Threading.Tasks;
using CF.Library.Core;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Library
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibraryWriter, IMusicLibraryReader
	{
		public async Task AddSong(Song song, string songSourceFileName)
		{
			await libraryStorage.AddSong(song, songSourceFileName);
			await UpdateSongChecksum(song, false);
			await libraryRepository.AddSong(song);
		}

		public async Task UpdateSong(Song song, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				await UpdateSongTagData(song, updatedProperties, false);
			}
			await libraryRepository.UpdateSong(song);
		}

		public async Task DeleteSong(Song song, DateTime deleteTime)
		{
			await libraryStorage.DeleteSong(song);

			song.DeleteDate = deleteTime;
			await libraryRepository.UpdateSong(song);
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await libraryStorage.ChangeDiscUri(disc, newDiscUri);
			disc.Uri = newDiscUri;
			await libraryRepository.UpdateDisc(disc);

			foreach (var song in disc.Songs)
			{
				song.Uri = libraryStructurer.ReplaceDiscPartInSongUri(newDiscUri, song.Uri);
				await libraryRepository.UpdateSong(song);
			}
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await libraryStorage.ChangeSongUri(song, newSongUri);
			song.Uri = newSongUri;
			await libraryRepository.UpdateSong(song);
		}

		public async Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				foreach (var song in disc.Songs)
				{
					await UpdateSongTagData(song, updatedProperties, true);
				}
			}
			await libraryRepository.UpdateDisc(disc);
		}

		public async Task DeleteDisc(Disc disc)
		{
			Application.Logger.WriteInfo($"Deleting disc '{disc.Title}'");
			var deleteTime = DateTime.Now;
			foreach (var song in disc.Songs)
			{
				await DeleteSong(song, deleteTime);
			}
			Application.Logger.WriteInfo($"Disc '{disc.Title}' was deleted successfully");
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			await libraryStorage.SetDiscCoverImage(disc, coverImageFileName);
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			await libraryRepository.AddSongPlayback(song, playbackTime);
		}

		public async Task FixSongTagData(Song song)
		{
			await libraryStorage.FixSongTagData(song);
			await UpdateSongChecksum(song, true);
		}

		private async Task UpdateSongTagData(Song song, UpdatedSongProperties updatedProperties, bool updateInRepository)
		{
			await libraryStorage.UpdateSongTagData(song, updatedProperties);
			await UpdateSongChecksum(song, updateInRepository);
		}

		private async Task UpdateSongChecksum(Song song, bool updateInRepository)
		{
			var songFileName = await libraryStorage.GetSongFile(song);
			song.Checksum = checksumCalculator.CalculateChecksumForFile(songFileName);

			if (updateInRepository)
			{
				await libraryRepository.UpdateSong(song);
			}
		}
	}
}
