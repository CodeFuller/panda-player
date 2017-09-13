using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibraryWriter, IMusicLibraryReader
	{
		public async Task AddSong(Song song, string songSourceFileName)
		{
			await libraryStorage.AddSong(song, songSourceFileName);
			await libraryRepository.AddSong(song);
		}

		public async Task UpdateSong(Song song, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				await libraryStorage.UpdateSongTagData(song, updatedProperties);
			}
			await libraryRepository.UpdateSong(song);
		}

		public async Task DeleteSong(Song song, DateTime deleteTime)
		{
			await libraryStorage.DeleteSong(song);

			song.DeleteDate = deleteTime;
			await libraryRepository.UpdateSong(song);
		}

		public async Task DeleteDisc(Disc disc)
		{
			var deleteTime = DateTime.Now;
			foreach (var song in disc.Songs)
			{
				await DeleteSong(song, deleteTime);
			}
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
		}
	}
}
