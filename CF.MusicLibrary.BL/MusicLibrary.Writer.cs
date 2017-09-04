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

		public async Task DeleteSong(Song song)
		{
			await libraryStorage.DeleteSong(song);
			await libraryRepository.DeleteSong(song);
		}

		public async Task DeleteDisc(Disc disc)
		{
			await libraryStorage.DeleteDisc(disc);
			await libraryRepository.DeleteDisc(disc);
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			await libraryStorage.SetDiscCoverImage(disc, coverImageFileName);
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			await libraryRepository.AddSongPlayback(song, playbackTime);
		}

		public async Task UpdateSongTagData(Song song, SongTagData tagData)
		{
			await libraryStorage.UpdateSongTagData(song, tagData);
		}

		public async Task FixSongTagData(Song song)
		{
			await libraryStorage.FixSongTagData(song);
		}
	}
}
